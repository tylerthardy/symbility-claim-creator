namespace SymbilityClaimCreator
{
    public class SymbilityClaimCreatorConfiguration
    {
        public SymbilityApiConfiguration ClaimSourceConfiguration { get; set; }
        public SymbilityApiConfiguration AssigneeConfiguration { get; set; }
        public SymbilityApiConfiguration SecondAssigneeConfiguration { get; set; }
    }

    public class SymbilityApiConfiguration
    {
        public string ApiUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
    }
}