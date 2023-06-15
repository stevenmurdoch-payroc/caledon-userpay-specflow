using System.Text.Json.Serialization;

namespace Common.Specs.Helpers;

public class Document
{
    [JsonPropertyName("purpose")]
    public string Purpose { get; set;}
    
    [JsonPropertyName("name")]
    public string Name { get; set;}
    
    [JsonPropertyName("mime-type")]
    public string MimeType { get; set;}
    
    [JsonPropertyName("description")]
    public string Description { get; set;}
}