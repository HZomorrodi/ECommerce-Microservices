using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace ProductsMicroService.API.APIEndpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", async (IProductService productService) =>
        {
            List<ProductResponse?> products = await productService.GetProducts();
            return Results.Ok(products);
        }
        );
        app.MapGet("/api/products/search/product-id/{ProductId:guid}", async (IProductService productService, Guid ProductId) =>
        {
            //await Task.Delay(100);
            //throw new NotImplementedException();
            ProductResponse? product = await productService.GetProductByCondition(p => p.ProductId == ProductId);
            if (product is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(product);
        }
        );
        app.MapGet("/api/products/search/{SearchString}", async (IProductService productService, string SearchString) =>
        {
            string searchPattern = $"%{SearchString}%";
            List<ProductResponse?> products = await productService.GetProductsByCondition(p =>
                (p.ProductName != null && EF.Functions.Like(p.ProductName, searchPattern)) ||
                (p.Category != null && EF.Functions.Like(p.Category, searchPattern))
            );
            return Results.Ok(products);
        }
        );
        app.MapPost("/api/products", async (IProductService productService, ProductAddRequest productAddRequest, IValidator<ProductAddRequest> productAddRequestValidator) =>
        {
            //validate the productAddRequest object using FluentValidation
            ValidationResult validationResult = await productAddRequestValidator.ValidateAsync(productAddRequest);
            if (!validationResult.IsValid)
            {
                IEnumerable<IGrouping<string, ValidationFailure>> rse = validationResult.Errors.GroupBy(x => x.PropertyName);
                Dictionary<string, string[]> errors = validationResult.Errors.GroupBy(x => x.PropertyName).
                   ToDictionary(x => x.Key, x => x.Select(grp => grp.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }
            ProductResponse? AddedProductResponse = await productService.AddProduct(productAddRequest);
            if (AddedProductResponse != null)
                return Results.Created($"/api/products/search/product-id/{AddedProductResponse.ProductId}", AddedProductResponse);
            return Results.Problem("Error in adding product");
        }
        );
        app.MapPut("/api/products", async (IProductService productService, ProductUpdateRequest productUpdateRequest, IValidator<ProductUpdateRequest> productUpdateRequestValidator) =>
        {
            //validate the productUpdateRequest object using FluentValidation
            ValidationResult validationResult = await productUpdateRequestValidator.ValidateAsync(productUpdateRequest);
            if (!validationResult.IsValid)
            {
                IEnumerable<IGrouping<string, ValidationFailure>> rse = validationResult.Errors.GroupBy(x => x.PropertyName);
                Dictionary<string, string[]> errors = validationResult.Errors.GroupBy(x => x.PropertyName).
                   ToDictionary(x => x.Key, x => x.Select(grp => grp.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }
            ProductResponse? UpdatedProductResponse = await productService.UpdateProduct(productUpdateRequest);
            if (UpdatedProductResponse != null)
                return Results.Ok(UpdatedProductResponse);
            return Results.Problem("Error in updating product");
        }
        );
        app.MapDelete("/api/products/{ProductId:guid}", async (IProductService productService, Guid ProductId) =>
        {
            bool isDeleted = await productService.DeleteProduct(ProductId);
            if (isDeleted)
            {
                return Results.Ok(isDeleted);
            }
            return Results.Problem("Error in deleting product");
        }
        );
        return app;
    }
}
