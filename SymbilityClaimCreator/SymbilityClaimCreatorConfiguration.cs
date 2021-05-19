using SymbilityClaimAccess.Models.Configuration;

namespace SymbilityClaimCreator
{
    public class SymbilityClaimCreatorConfiguration
    {
        public SymbilityApiConfiguration ClaimSourceConfiguration { get; set; }
        public SymbilityApiConfiguration AssigneeConfiguration { get; set; }
        public SymbilityApiConfiguration SecondAssigneeConfiguration { get; set; }
    }
}