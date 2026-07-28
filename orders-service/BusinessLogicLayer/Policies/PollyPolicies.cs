using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Policies
{
    public class PollyPolicies(ILogger<PollyPolicies> logger) : IPollyPolicies
    {
        private readonly ILogger<PollyPolicies> logger = logger;

        public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
        {
            AsyncRetryPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).
                                    WaitAndRetryAsync(retryCount: retryCount,
                                         sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                         onRetry: (outcome, timespan, retryAttempt, context) =>
                                         {
                                             logger.LogInformation($"Retry {retryAttempt} after " +
                                                 $"{timespan.TotalSeconds} seconds {DateTime.Now}");
                                         });
            return policy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak)
        {
            AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).
                                    CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: handledEventsAllowedBeforeBreaking,
                                                        durationOfBreak: durationOfBreak,
                                                        onBreak: (outcoume, timeSpan) =>
                                                        {
                                                            logger.LogInformation($"Circuit Breaker Opened for {timeSpan.TotalMinutes} minutes " +
                                                 $"due to consecutive 3 failures. The subsequent requests will be blocked. {DateTime.Now}");
                                                        },
                                                        onReset: () =>
                                                        {
                                                            logger.LogInformation($"Circuit Breaker Closed.The subsequent requests will be allowed. {DateTime.Now}");
                                                        });
            return policy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout)
        {
            AsyncTimeoutPolicy<HttpResponseMessage> policy = Policy.TimeoutAsync<HttpResponseMessage>(timeout);

            return policy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetBulkheadIsolationPolicy()
        {
            return Policy.BulkheadAsync<HttpResponseMessage>(maxParallelization: 2,
                                                             maxQueuingActions: 40,
                                                             onBulkheadRejectedAsync: context =>
                                                             {
                                                                 logger.LogWarning("BulkheadIsolation triggered. Can't send any more requests since the queue is full");
                                                                 throw new BulkheadRejectedException("Bulkhead queue is full");
                                                             });
        }

        public IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
        {
            AsyncFallbackPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).
                                    FallbackAsync(async cts =>
                                    {
                                        logger.LogWarning("Fallback triggered: The request failed, returning dummy data");
                                        ProductDTO productDTO = new(ProductId: default,
                                                                    ProductName: "Temporarily Unavailable (fallback)",
                                                                    Category: "Temporarily Unavailable (fallback)",
                                                                    UnitPrice: default,
                                                                    QuantityInStock: default);
                                        HttpResponseMessage httpResponseMessage = new(HttpStatusCode.ServiceUnavailable)
                                        { Content = new StringContent(JsonSerializer.Serialize(productDTO), Encoding.UTF8, MediaTypeNames.Application.Json) };
                                        return httpResponseMessage;
                                    });
            return policy;
        }
    }
}
