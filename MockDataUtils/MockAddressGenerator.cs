using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        public MockAddress GetRandomAddress()
        {
            var random = new Random();
            var index = random.Next(0, _mockAddresses.Length);
            return _mockAddresses[index];
        }
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
