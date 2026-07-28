using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories;

public class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{
    private readonly ApplicationDbContext dbContext = dbContext;

    public async Task<Product?> AddProduct(Product product)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProduct(Guid productId)
    {
        int rowsAffected = await dbContext.Products.Where(x => x.ProductId == productId).ExecuteDeleteAsync();
        return rowsAffected > 0;
    }

    public async Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        return await dbContext.Products.FirstOrDefaultAsync(conditionExpression);
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await dbContext.Products.ToListAsync();
    }

    public async Task<IEnumerable<Product?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        return await dbContext.Products.Where(conditionExpression).ToListAsync();
    }

    public async Task<Product?> UpdateProduct(Product product)
    {
        int rowsAffected = await dbContext.Products
            .Where(x => x.ProductId == product.ProductId)
            .ExecuteUpdateAsync(setters => setters
            .SetProperty(p => p.ProductName, product.ProductName)
            .SetProperty(p => p.Category, product.Category)
            .SetProperty(p => p.UnitPrice, product.UnitPrice)
            .SetProperty(p => p.QuantityInStock, product.QuantityInStock)
        );

        return rowsAffected > 0 ? product : null;
    }
}


