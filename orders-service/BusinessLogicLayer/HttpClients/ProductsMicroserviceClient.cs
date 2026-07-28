using BusinessLogicLayer.DTO;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.Bulkhead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogicLayer.HttpClients
{
    public class ProductsMicroserviceClient(HttpClient httpClient, ILogger<ProductsMicroserviceClient> logger, IDistributedCache distributedCache)
    {
        private readonly HttpClient httpClient = httpClient;
        private readonly ILogger<ProductsMicroserviceClient> logger = logger;
        private readonly IDistributedCache distributedCache = distributedCache;

        public async Task<ProductDTO?> GetProductsByProuductID(Guid productID)
        {
            try
            {
                string cacheKey = $"product:{productID}";
                string? cachedProduct = await distributedCache.GetStringAsync(cacheKey);
                if (cachedProduct is not null)
                {
                    return JsonSerializer.Deserialize<ProductDTO>(cachedProduct);
                }
                HttpResponseMessage response = await httpClient.GetAsync($"/gateway/products/search/product-id/{productID}");
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                    {
                        ProductDTO? productFromFallback = await response.Content.ReadFromJsonAsync<ProductDTO>() ?? throw new NotImplementedException("Fallback policy was not implemented");
                        return productFromFallback;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return null;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        throw new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
                    }
                }
                ProductDTO? product = await response.Content.ReadFromJsonAsync<ProductDTO>() ?? throw new ArgumentException("Invalid product ID");
                string productJson = JsonSerializer.Serialize(product);
                DistributedCacheEntryOptions distributedCacheEntryOptions = new()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(300),
                    SlidingExpiration = TimeSpan.FromSeconds(100)
                };

                await distributedCache.SetStringAsync(cacheKey, productJson, distributedCacheEntryOptions);
                return product;
            }
            catch (BulkheadRejectedException ex)
            {
                logger.LogError(ex, "Bulkhead isolation blocks the request since the request queue is full");
                return new ProductDTO(ProductId: default,
                                      ProductName: "Temporarily Unavailable (Bulkhead)",
                                      Category: "Temporarily Unavailable (Bulkhead)",
                                      UnitPrice: default,
                                      QuantityInStock: default);
            }
        }
    }
}
