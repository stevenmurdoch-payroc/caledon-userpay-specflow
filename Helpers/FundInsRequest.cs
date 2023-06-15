using System;
using System.Text.Json.Serialization;

namespace Common.Specs.Helpers;

public class FundInsRequest
{
    [JsonPropertyName("recipientId")]
    public string RecipientId { get; set;}
}