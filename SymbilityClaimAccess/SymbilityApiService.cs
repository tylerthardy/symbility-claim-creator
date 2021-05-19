using System.Threading;
using System.Threading.Tasks;
using Namotion.Reflection;
using SymbilityClaimAccess.Models.Configuration;

namespace SymbilityClaimAccess
{
    public class SymbilityApiService
    {
        private readonly SymbilityRestClient _symbilityRestClient;

        public SymbilityApiService(SymbilityApiConfiguration configuration)
        {
            var httpClient = new SymbilityApiHttpClient(configuration);
            _symbilityRestClient = new SymbilityRestClient(httpClient);
        }

        public async Task<object> CreateClaim(ClaimSpecification claimSpecification, CancellationToken cancellationToken)
        {
            claimSpecification.EnsureValidNullability();
            return await _symbilityRestClient.ClaimCreateClaimAsync(null, claimSpecification, cancellationToken);
        }
        
    }
}
