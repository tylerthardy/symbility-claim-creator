using System.Threading;
using System.Threading.Tasks;
using SymbilityClaimAccess.Models.Configuration;
using SymbilityClaimAccess.Models.Extensions;

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

        public async Task<Claim> CreateClaim(ClaimSpecification claimSpecification, string fromUserSpecification, CancellationToken? cancellationToken = null)
        {
            claimSpecification.AssertValid();
            return await _symbilityRestClient.ClaimCreateClaimAsync(fromUserSpecification, claimSpecification, cancellationToken ?? CancellationToken.None);
        }

        public async Task<ClaimAssignment> AssignClaim(Claim claim, ClaimAssignment parentAssignment, AddClaimAssigneeSpecification addClaimAssigneeSpecification, string fromUserSpecification, CancellationToken? cancellationToken = null)
        {
            return await _symbilityRestClient.AssignmentAddClaimAssigneeAsync(
                claim.UniqueID,
                parentAssignment?.AssignmentID ?? 0,
                fromUserSpecification,
                addClaimAssigneeSpecification,
                cancellationToken ?? CancellationToken.None
            );
        }
    }
}
