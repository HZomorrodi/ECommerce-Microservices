using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.RabbitMQ;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services;

public class ProductService(IProductRepository productRepository,
                            IMapper mapper,
                            IRabbitMQPublisher rabbitMQPublisher,
                            IValidator<ProductAddRequest> productAddRequestValidator,
                            IValidator<ProductUpdateRequest> productUpdateRequestValidator) : IProductService
{

    public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
    {
        ArgumentNullException.ThrowIfNull(productAddRequest);

        //Validate the product using Fluent Validation
        ValidationResult validation = await productAddRequestValidator.ValidateAsync(productAddRequest);

        //Check the validation result
        if (!validation.IsValid)
        {
            string errors = string.Join(",", validation.Errors.Select(v => v.ErrorMessage));
            throw new ArgumentException(errors);
        }

        Product productInput = mapper.Map<Product>(productAddRequest);
        Product? addedProduct = await productRepository.AddProduct(productInput);
        if (addedProduct is null)
            return null;
        return mapper.Map<ProductResponse>(addedProduct);
    }

    public async Task<bool> DeleteProduct(Guid productId)
    {
        Product? existingProduct = await productRepository.GetProductByCondition(p => p.ProductId == productId);
        if (existingProduct is null)
            return false;
        bool isDeleted = await productRepository.DeleteProduct(productId);
        if (isDeleted)
        {
            ProductDeletionMessage message = new(productId, existingProduct.ProductName);
            string routingKey = "product.delete";
            //await rabbitMQPublisher.Publish(routingKey, message);
            await rabbitMQPublisher.Publish(
                new Dictionary<string, object>
                {
                    ["event"] = "delete",
                    ["entity"] = "product"
                },
                message);
        }
        return isDeleted;
    }

    public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        Product? product = await productRepository.GetProductByCondition(conditionExpression);
        if (product is null)
        {
            return null;
        }
        return mapper.Map<ProductResponse>(product);
    }

    public async Task<List<ProductResponse?>> GetProducts()
    {
        IEnumerable<Product> products = await productRepository.GetProducts();
        return mapper.Map<IEnumerable<ProductResponse>>(products).ToList();
    }

    public async Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        IEnumerable<Product?> products = await productRepository.GetProductsByCondition(conditionExpression);
        return mapper.Map<IEnumerable<ProductResponse?>>(products).ToList();
    }

    public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
    {
        Product existingProduct = await productRepository.GetProductByCondition(p => p.ProductId == productUpdateRequest.ProductId) ?? throw new ArgumentException("Invalid Product Id");

        //Validate the product using Fluent Validation
        ValidationResult validation = await productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

        //Check the validation result
        if (!validation.IsValid)
        {
            string errors = string.Join(",", validation.Errors.Select(v => v.ErrorMessage));
            throw new ArgumentException(errors);
        }

        Product productInput = mapper.Map<Product>(productUpdateRequest);
        Product? updatedProduct = await productRepository.UpdateProduct(productInput);
        
        if (updatedProduct is null)
            return null;

        //if (existingProduct.ProductName != updatedProduct.ProductName)
        {
            ProductNameUpdateMessage message = new(updatedProduct.ProductId, updatedProduct.ProductName);
            string routingKey = "product.update.name";
            await rabbitMQPublisher.Publish(
                     new Dictionary<string, object>
                     {
                         ["event"] = "update",
                         ["entity"] = "product",
                         ["field"] = "name"
                     },
                     productInput);
        }

        return mapper.Map<ProductResponse>(updatedProduct);
    }
}
