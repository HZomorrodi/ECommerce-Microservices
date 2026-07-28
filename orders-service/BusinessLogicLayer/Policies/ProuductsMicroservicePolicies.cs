using Polly;
using Polly.Wrap;

namespace BusinessLogicLayer.Policies
{
    public class ProuductsMicroservicePolicies(IPollyPolicies pollyPolicies) : IProuductsMicroservicePolicies
    {
        private readonly IPollyPolicies pollyPolicies = pollyPolicies;

        public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
        {
            IAsyncPolicy<HttpResponseMessage> fallbackPolicy = pollyPolicies.GetFallbackPolicy();
            IAsyncPolicy<HttpResponseMessage> bulkheadIsolationPolicy = pollyPolicies.GetBulkheadIsolationPolicy();

            AsyncPolicyWrap<HttpResponseMessage> wrappedPolicy = Policy.WrapAsync(fallbackPolicy, bulkheadIsolationPolicy);
            return wrappedPolicy;
        }


    }
}
