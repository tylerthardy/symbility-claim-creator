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

        public ClaimCreator(ClaimCreatorConfiguration configuration, MockAddressGenerator mockAddressGenerator)
        {
            _mockAddressGenerator = mockAddressGenerator;

            _sourceConfiguration = configuration.ClaimSourceConfiguration;
            _firstAssigneeConfiguration = configuration.FirstAssigneeConfiguration;
            _secondAssigneeConfiguration = configuration.SecondAssigneeConfiguration;

            _claimSourceApiService = new SymbilityApiService(_sourceConfiguration);
        }

        public async Task<Claim> CreateSourceClaim()
        {
            var claimNumber = "THAuto-" + DateTime.UtcNow.ToString("MMddyyy-HHMM");
            var claimSpec = GenerateClaimSpec(claimNumber);
            return await CreateSourceClaim(claimSpec);
        }

        public async Task<Claim> CreateSourceClaim(ClaimSpecification claimSpecification)
        {
            var claim = await _claimSourceApiService.CreateClaim(claimSpecification);
            return claim;
        }

        public async Task<ClaimAssignment> AssignToFirstAssignee(Claim claim)
        {
            var fromSourceUserSpec = _firstAssigneeConfiguration.FromUserSpecification;
            var assignmentTypeCode = "ONSITE-SVCS";
            var firstAssigneeSpec = GenerateAssigneeSpec(_firstAssigneeConfiguration.CompanyId, assignmentTypeCode);
            var firstAssigneeAssignment =
                await _claimSourceApiService.AssignClaim(claim, null, firstAssigneeSpec, fromSourceUserSpec);
            return firstAssigneeAssignment;
        }

        public async Task<ClaimAssignment> AssignToSecondAssignee(Claim claim, ClaimAssignment assignment)
        {
            var fromFirstUserSpec = _firstAssigneeConfiguration.FromUserSpecification;
            var secondAssigneeSpec = GenerateAssigneeSpec(_secondAssigneeConfiguration.CompanyId, assignmentTypeCode: null);
            var secondAssigneeAssignment = await _claimSourceApiService.AssignClaim(claim, assignment, secondAssigneeSpec, fromFirstUserSpec);
            return secondAssigneeAssignment;
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
