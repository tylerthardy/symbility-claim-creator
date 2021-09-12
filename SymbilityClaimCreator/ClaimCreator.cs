using System;
using System.Threading.Tasks;
using MockDataUtils;
using SymbilityClaimAccess;
using SymbilityClaimAccess.Models.Configuration;
using SymbilityClaimAccess.Models.Extensions;
using TimeZone = SymbilityClaimAccess.TimeZone;

namespace SymbilityClaimCreator
{
    public class ClaimCreator
    {
        private readonly MockAddressGenerator _mockAddressGenerator;
        private SymbilityApiService _claimSourceApiService;
        private SymbilityApiConfiguration _sourceConfiguration;
        private SymbilityApiConfiguration _firstAssigneeConfiguration;
        private SymbilityApiConfiguration _secondAssigneeConfiguration;
        private SymbilityApiService _firstAssigneeApiService;

        public ClaimCreator(ClaimCreatorConfiguration configuration, MockAddressGenerator mockAddressGenerator)
        {
            _mockAddressGenerator = mockAddressGenerator;

            _sourceConfiguration = configuration.ClaimSourceConfiguration;
            _firstAssigneeConfiguration = configuration.FirstAssigneeConfiguration;
            _secondAssigneeConfiguration = configuration.SecondAssigneeConfiguration;

            _claimSourceApiService = new SymbilityApiService(_sourceConfiguration);
            _firstAssigneeApiService = new SymbilityApiService(_firstAssigneeConfiguration);
        }

        public async Task<Claim> CreateSourceClaim()
        {
            var claimNumber = "THAuto-" + DateTime.UtcNow.ToString("MMddyyy-HHmm");
            var claimSpec = GenerateClaimSpec(claimNumber);
            return await CreateSourceClaim(claimSpec);
        }

        public async Task<Claim> CreateSourceClaim(ClaimSpecification claimSpecification)
        {
            var claim = await _claimSourceApiService.CreateClaim(claimSpecification, _sourceConfiguration.FromUserSpecification);
            return claim;
        }

        public async Task<ClaimAssignment> AssignToFirstAssignee(Claim claim)
        {
            var userSpec = _sourceConfiguration.FromUserSpecification;
            var assignmentTypeCode = "ONSITE-SVCS";
            var assigneeSpec = GenerateAssigneeSpec(_firstAssigneeConfiguration.CompanyId, assignmentTypeCode);
            var assignment = await _claimSourceApiService.AssignClaim(claim, null, assigneeSpec, userSpec);
            return assignment;
        }

        public async Task<ClaimAssignment> AssignToSecondAssignee(Claim claim, ClaimAssignment assignment)
        {
            var userSpec = _firstAssigneeConfiguration.FromUserSpecification;
            var assigneeSpec = GenerateAssigneeSpec(_secondAssigneeConfiguration.CompanyId, assignmentTypeCode: null);
            var secondAssignment = await _firstAssigneeApiService.AssignClaim(claim, assignment, assigneeSpec, userSpec);
            return secondAssignment;
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
