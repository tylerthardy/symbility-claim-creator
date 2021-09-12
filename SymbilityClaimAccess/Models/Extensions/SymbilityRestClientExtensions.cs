using System;
using Namotion.Reflection;

namespace SymbilityClaimAccess.Models.Extensions
{
    public static class SymbilityRestClientExtensions
    {
        public static void AssertValid(this ClaimSpecification claim)
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

            // Null validity
            claim.EnsureValidNullability();
        }
    }
}
