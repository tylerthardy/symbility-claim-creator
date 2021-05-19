﻿using SymbilityClaimAccess.Models.Configuration;

namespace SymbilityClaimCreator
{
    public class ClaimCreatorConfiguration
    {
        public SymbilityApiConfiguration ClaimSourceConfiguration { get; set; }
        public SymbilityApiConfiguration AssigneeConfiguration { get; set; }
        public SymbilityApiConfiguration SecondAssigneeConfiguration { get; set; }
    }
}