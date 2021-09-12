using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MockDataUtils;
using SymbilityClaimAccess;
using SymbilityClaimAccess.Models.Configuration;
using SymbilityClaimAccess.Models.Extensions;
using Task = System.Threading.Tasks.Task;
using TimeZone = SymbilityClaimAccess.TimeZone;

namespace SymbilityClaimCreator
{
    public class ClaimCreator : IHostedService
    {
        private readonly MockAddressGenerator _mockAddressGenerator;
        private SymbilityApiService _claimSourceApiService;
        private SymbilityApiConfiguration _sourceConfiguration;
        private SymbilityApiConfiguration _firstAssigneeConfiguration;
        private SymbilityApiConfiguration _secondAssigneeConfiguration;

        public ClaimCreator(IOptions<ClaimCreatorConfiguration> options, MockAddressGenerator mockAddressGenerator)
        {
            _mockAddressGenerator = mockAddressGenerator;

            var configuration = options.Value;
            _sourceConfiguration = configuration.ClaimSourceConfiguration;
            _firstAssigneeConfiguration = configuration.FirstAssigneeConfiguration;
            _secondAssigneeConfiguration = configuration.SecondAssigneeConfiguration;

            _claimSourceApiService = new SymbilityApiService(_sourceConfiguration);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create claim
            var claimNumber = "THAuto-" + DateTime.UtcNow.ToString("MMddyyy-HHMM");
            var claimSpec = GenerateClaimSpec(claimNumber);
            var claim = await _claimSourceApiService.CreateClaim(claimSpec, cancellationToken);

            // Assign to first (IA)
            var fromSourceUserSpec = _firstAssigneeConfiguration.FromUserSpecification;
            var assignmentTypeCode = "ONSITE-SVCS";
            var firstAssigneeSpec = GenerateAssigneeSpec(_firstAssigneeConfiguration.CompanyId, assignmentTypeCode);
            var firstAssigneeAssignment = await _claimSourceApiService.AssignClaim(claim, null, firstAssigneeSpec, fromSourceUserSpec, cancellationToken);

            // Assign to second (adjuster)
            var fromFirstUserSpec = _firstAssigneeConfiguration.FromUserSpecification;
            var secondAssigneeSpec = GenerateAssigneeSpec(_secondAssigneeConfiguration.CompanyId, assignmentTypeCode);
            var secondAssigneeAssignment = await _claimSourceApiService.AssignClaim(claim, firstAssigneeAssignment, secondAssigneeSpec, fromFirstUserSpec, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        private ClaimSpecification GenerateClaimSpec(string claimNumber)
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
                    State = mockAddress.State.GetStateCode()
                }
            };
            
            claim.AssertValid();
            return claim;
        }

        private AddClaimAssigneeSpecification GenerateAssigneeSpec(string companyId, string assignmentTypeCode)
        {
            return new AddClaimAssigneeSpecification()
            {

                AssigneeCompanyIDSpecification = new CompanyIDSpecification()
                {
                    CompanyID = companyId,
                    CompanyIDType = CompanyIDType.CompanyID
                },
                AssignmentNotes = "Automatically assigned claim from testing harness",
                AssignmentTypeCode = assignmentTypeCode
            };
        }
    }
}
