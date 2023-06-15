using System;
using System.Text.Json.Serialization;

namespace Common.Specs.Helpers;

public class AddressJSON
{
    [JsonPropertyName("address1")]
    public string Address1 { get; set; }

    [JsonPropertyName("address2")]
    public string? Address2 { get; set; }

    [JsonPropertyName("address3")]
    public string? Address3 { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("postalCode")]
    public string PostalCode { get; set; }
    
}