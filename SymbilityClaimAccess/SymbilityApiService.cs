using System.Threading;
using System.Threading.Tasks;
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

        public async Task<Claim> CreateClaim(ClaimSpecification claimSpecification, CancellationToken cancellationToken)
        {
            return await _symbilityRestClient.ClaimCreateClaimAsync(null, claimSpecification, cancellationToken);
        }

        public async Task<ClaimAssignment> AssignClaim(Claim claim, ClaimAssignment parentAssignment, AddClaimAssigneeSpecification addClaimAssigneeSpecification, string fromUserSpecification, CancellationToken cancellationToken)
        {
            return await _symbilityRestClient.AssignmentAddClaimAssigneeAsync(
                claim.UniqueID,
                parentAssignment?.AssignmentID ?? 0,
                fromUserSpecification,
                addClaimAssigneeSpecification,
                cancellationToken
            );
        }
    }
}
