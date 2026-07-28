using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServiceContracts;

public interface IProductService
{
    /// <summary>
    /// Retreive the list of products from the product repository
    /// </summary>
    /// <returns>Returns list of ProductResponse objects</returns>
    Task<List<ProductResponse?>> GetProducts();
    /// <summary>
    /// Retreives list of products matching with given condition 
    /// </summary>
    /// <param name="conditionExpression">Expression that represent 
    /// condition to check</param>
    /// <returns>Returns matching products</returns>
    Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression);
    /// <summary>
    /// Returns a single product that matches with given condition
    /// </summary>
    /// <param name="conditionExpression">Express that represent
    /// condition to check</param>
    /// <returns>Returns matching product or null</returns>
    Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression);
    /// <summary>
    /// Adds (inserts) product into the table using products repository
    /// </summary>
    /// <param name="productAddRequest">Product to insert</param>
    /// <returns>Product after inserting or null if unsuccessful</returns>
    Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest);
    /// <summary>
    /// Updates the existing product based on the ProductId
    /// </summary>
    /// <param name="productUpdateRequest">Product data to update</param>
    /// <returns>Returns product object after successful updation;
    /// otherwise null</returns>
    Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest);
    /// <summary>
    /// Deletes an existing product based on ProductId
    /// </summary>
    /// <param name="productId">ProductId to search and delete</param>
    /// <returns>Returns true if deletion is successful;
    /// otherwise fasle</returns>
    Task<bool> DeleteProduct(Guid productId);

}
