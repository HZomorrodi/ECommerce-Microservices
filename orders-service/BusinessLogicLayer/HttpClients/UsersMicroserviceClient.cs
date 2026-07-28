using Amazon.Runtime.Internal.Util;
using BusinessLogicLayer.DTO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLogicLayer.HttpClients;

public class UsersMicroserviceClient(HttpClient httpClient, ILogger<UsersMicroserviceClient> logger, IDistributedCache distributedCache)
{
    private readonly HttpClient httpClient = httpClient;
    private readonly ILogger<UsersMicroserviceClient> logger = logger;
    private readonly IDistributedCache distributedCache = distributedCache;

    public async Task<UserDTO?> GetUserByUserID(Guid userID)
    {
        try
        {
            string cacheKey = $"user:{userID}";
            string? cachedUser = await distributedCache.GetStringAsync(cacheKey);
            if (cachedUser is not null)
            {
                return JsonSerializer.Deserialize<UserDTO>(cachedUser);
            }
            HttpResponseMessage response = await httpClient.GetAsync($"/gateway/users/{userID}");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    UserDTO fallbackUser = await response.Content.ReadFromJsonAsync<UserDTO>() ?? throw new NotImplementedException("Fallback policy was not implemented");
                    return fallbackUser;
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
                    return new UserDTO(
                        UserID: default,
                        Email: "Temporary Unavailable",
                        Gender: "Temporary Unavailable",
                        PersonName: "Temporary Unavailable"
                    );
                    throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
                }
            }
            UserDTO user = await response.Content.ReadFromJsonAsync<UserDTO>() ?? throw new ArgumentException("Invalid User ID");
            string userJson = JsonSerializer.Serialize(user);
            DistributedCacheEntryOptions distributedCacheEntryOptions = new()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(3),
            };

            await distributedCache.SetStringAsync(cacheKey, userJson, distributedCacheEntryOptions); 
            return user;
        }
        catch (BrokenCircuitException ex) 
        {
            logger.LogError(ex, "Request failed because of circuit breaker is in open state. Returning dummy");
            return new UserDTO(
                       UserID: default,
                       Email: "Temporary Unavailable (circuit breaker)",
                       Gender: "Temporary Unavailable (circuit breaker)",
                       PersonName: "Temporary Unavailable (circuit breaker)"
                   );
        }
        catch (TimeoutRejectedException ex)
        {
                logger.LogError(ex, $"Timeout occured while fetching user data. Returning dummy data1 {DateTime.Now}");
            return new UserDTO(
                       UserID: default,
                       Email: "Temporary Unavailable (timeout)",
                       Gender: "Temporary Unavailable (timeout)",
                       PersonName: "Temporary Unavailable (timeout)"
                   );
        }
    }
}

