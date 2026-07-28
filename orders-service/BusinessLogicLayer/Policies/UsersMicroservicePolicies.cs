using Polly;
using Polly.Wrap;

namespace BusinessLogicLayer.Policies
{
    public class UsersMicroservicePolicies( IPollyPolicies pollyPolicies) : IUsersMicroservicePolicies
    {
        private readonly IPollyPolicies pollyPolicies = pollyPolicies;

        public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
        {
            IAsyncPolicy<HttpResponseMessage> retryPolicy = pollyPolicies.GetRetryPolicy(5);
            IAsyncPolicy<HttpResponseMessage> circuitBreakerPolicy = pollyPolicies.GetCircuitBreakerPolicy(3, TimeSpan.FromMinutes(2));
            IAsyncPolicy<HttpResponseMessage> timeoutPolicy = pollyPolicies.GetTimeoutPolicy(TimeSpan.FromMilliseconds(1500));

            AsyncPolicyWrap<HttpResponseMessage> wrappedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
            return wrappedPolicy;
        }
    }
}
