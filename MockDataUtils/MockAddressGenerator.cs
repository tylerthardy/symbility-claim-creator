using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetXtensions.Globalization;

namespace MockDataUtils
{
    public class MockAddressGenerator
    {
        private const string AddressSourceJson = "addresses-us-all.min.json";

        private readonly MockAddress[] _mockAddresses;

        public MockAddressGenerator()
        {
            var sourceJson = File.ReadAllText(AddressSourceJson);
            _mockAddresses = JsonSerializer.Deserialize<MockAddressSource>(sourceJson).Addresses;
        }

        public MockAddress GetRandomAddress(AddressOptions options)
        {
            var random = new Random();
            var addresses = ApplyFilters(_mockAddresses, options);
            var index = random.Next(0, addresses.Length);
            return ApplyOptions(addresses[index], options);
        }

        private MockAddress[] ApplyFilters(MockAddress[] addresses, AddressOptions options)
        {
            var results = addresses.AsQueryable();
            if (!string.IsNullOrEmpty(options.StateCode)) results = results.Where(a => a.State == options.StateCode);

            return results.ToArray();
        }

        private MockAddress ApplyOptions(MockAddress address, AddressOptions options)
        {
            if (options.LongStateNames) address.State = LookupStateName(address.State);

            return address;
        }

        private string LookupStateName(string stateCode)
        {
            if (!GeoNames.USCanadaStatesByAbbreviationDict.TryGetValue(stateCode, out var stateName))
            {
                throw new InvalidDataException($"No State fullname found for abbreviation: {stateCode}");
            }

            return stateName;
        }
    }

    public class AddressOptions
    {
        public bool LongStateNames { get; set; }
        public string StateCode { get; set; }
    }

    public class MockAddressSource
    {
        [JsonPropertyName("addresses")]
        public MockAddress[] Addresses { get; set; }
    }

    public class Coordinates
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lng")]
        public double Lng { get; set; }
    }

    public class MockAddress
    {
        [JsonPropertyName("address1")]
        public string Address1 { get; set; }

        [JsonPropertyName("address2")]
        public string Address2 { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("coordinates")]
        public Coordinates Coordinates { get; set; }
    }




}
