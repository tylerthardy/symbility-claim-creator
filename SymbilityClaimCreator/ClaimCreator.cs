using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SymbilityClaimAccess;
using Task = System.Threading.Tasks.Task;

namespace SymbilityClaimCreator
{
    public class ClaimCreator : IHostedService
    {
        private SymbilityApiService _claimSourceApiService;

        public ClaimCreator(IOptions<ClaimCreatorConfiguration> options)
        {
            var configuration = options.Value;
            _claimSourceApiService = new SymbilityApiService(configuration.ClaimSourceConfiguration);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var claimNumber = "TH-AutoCreationTest" + DateTime.UtcNow.ToString("MMddyyy-HHMM");
            var claim = CreateClaim(claimNumber);
            await _claimSourceApiService.CreateClaim(claim, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        private ClaimSpecification CreateClaim(string claimNumber)
        {
            var policyNumber = "POL-" + claimNumber;
            var claim = new ClaimSpecification
            {
                Number = claimNumber,
                InsuredCompanyName = "TestCompany",
                PolicyNumber = policyNumber,
                LossType = "fire",
                LossDate = DateTimeOffset.UtcNow.AddDays(-1),
                InsuredAddress = new Address
                {
                    City = "test",
                    Line1 = "test2",
                    ZipCode = "12312"
                }
            };
            
            ValidateClaim(claim);
            return claim;
        }

        private void ValidateClaim(ClaimSpecification claim)
        {
            //TODO: Determine better way to perform realistic validation on generated API
            var invalidException = new InvalidOperationException("The object's nullability is invalid: Custom validation");

            // Missing common-sense validations
            if (string.IsNullOrEmpty(claim.Number)) throw invalidException;
            if (string.IsNullOrEmpty(claim.InsuredCompanyName) &&
                (string.IsNullOrEmpty(claim.InsuredFirstName) || string.IsNullOrEmpty(claim.InsuredLastName)))
                throw invalidException;
            if (string.IsNullOrEmpty(claim.PolicyNumber)) throw invalidException;
            if (claim.LossDate.Equals(default)) throw invalidException;

            // Client specific validations
            if (string.IsNullOrEmpty(claim.LossType)) throw invalidException;
            if (claim.InsuredAddress == null || string.IsNullOrEmpty(claim.InsuredAddress.Line1) ||
                string.IsNullOrEmpty(claim.InsuredAddress.City) ||
                string.IsNullOrEmpty(claim.InsuredAddress.ZipCode)) throw invalidException;
        }
    }
}
