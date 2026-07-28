using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.RepositoryContracts;

/// <summary>
/// Represent a repository for managing 'product' table
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retrieves all products asynchronously
    /// </summary>
    /// <returns>Returns all Products from the table</returns>
    Task<IEnumerable<Product>> GetProducts();
    /// <summary>
    /// Retrieves all products based on specified 
    /// condition asynchronously.
    /// </summary>
    /// <param name="conditionExpression">The condition to 
    /// filter products</param>
    /// <returns>Returning a collection of matching products</returns>
    Task<IEnumerable<Product?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression);
    /// <summary>
    /// Retrieves a single product based on specified 
    /// condition asynchronously.
    /// </summary>
    /// <param name="conditionExpression">The condition to 
    /// filter products</param>
    /// <returns>Returns a single product or null if not found</returns>
    Task<Product?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression);
    /// <summary>
    /// Adds a new product into the products table asynchronously
    /// </summary>
    /// <param name="product">The product to be added</param>
    /// <returns>Returns the added product object
    /// or null if unsuccessful</returns>
    Task<Product?> AddProduct(Product product);
    /// <summary>
    /// Updates an existing product asynchronously
    /// </summary>
    /// <param name="product">The product to be updated</param>
    /// <returns>Returns the updated product; or null if
    /// not found</returns>
    Task<Product?> UpdateProduct(Product product);
    /// <summary>
    /// Deletes the product asynchronously
    /// </summary>
    /// <param name="productId">The product ID to be deleted</param>
    /// <returns>Returns true if the deletion is successful,
    /// false otherwise
    /// </returns>
    Task<bool> DeleteProduct(Guid productId);
}
