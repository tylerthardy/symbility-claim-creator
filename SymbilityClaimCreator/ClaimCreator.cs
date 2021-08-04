using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MockDataUtils;
using SymbilityClaimAccess;
using Task = System.Threading.Tasks.Task;
using TimeZone = SymbilityClaimAccess.TimeZone;

namespace SymbilityClaimCreator
{
    public class ClaimCreator : IHostedService
    {
        private readonly MockAddressGenerator _mockAddressGenerator;
        private SymbilityApiService _claimSourceApiService;

        public ClaimCreator(IOptions<ClaimCreatorConfiguration> options, MockAddressGenerator mockAddressGenerator)
        {
            _mockAddressGenerator = mockAddressGenerator;
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
            var mockAddress = _mockAddressGenerator.GetRandomAddress(new AddressOptions { LongStateNames = true, StateCode = "DC"});
            var claim = new ClaimSpecification
            {
                CreationDate = DateTimeOffset.UtcNow,
                Number = claimNumber,
                InsuredCompanyName = "TestCompany",
                PolicyNumber = policyNumber,
                LossType = "fire",
                LossDate = DateTimeOffset.UtcNow.AddDays(-1),
                LossDateTimeZone = TimeZone.Utc,
                InsuredAddress = new Address
                {
                    City = mockAddress.City,
                    Line1 = mockAddress.Address1,
                    ZipCode = mockAddress.PostalCode,
                    State = GetStateCode(mockAddress.State)
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

        private State GetStateCode(string state)
        {
            if (state == "Washington DC") return State.DistrictOfColumbia;
            var stateEnum = Enum.Parse<State>(state);
            return stateEnum;
        }
    }
}
