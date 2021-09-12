using System;

namespace SymbilityClaimAccess.Models.Extensions
{
    public static class StringExtensions
    {
        public static State GetStateCode(this string state)
        {
            if (state == "Washington DC") return State.DistrictOfColumbia;
            var stateEnum = Enum.Parse<State>(state);
            return stateEnum;
        }
    }
}
