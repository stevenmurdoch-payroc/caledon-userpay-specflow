using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers;
using TechTalk.SpecFlow;

namespace PaymentAPI.Drivers;

public class RequestDriver
{
    private readonly ScenarioContext _scenarioContext;
    private string _resourceLocation;
    
    //A list of all responses received during the current test
    public readonly List<RestResponse> Responses = new();

    //This should be set to the URL of the next intended HttpRequest
    public string ResourceLocation
    {
        get => _resourceLocation;
        set => _resourceLocation = value;
    }

    public RequestDriver(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }
    
    /// <summary>
    /// Performs a post request using the ResourceLocation property for the URL
    /// </summary>
    /// <returns></returns>
    public async Task<RestResponse> PerformPost()
    {
        var apiUrl = _scenarioContext["API_URL"].ToString();
        var jsonString = _scenarioContext["payload"].ToString();
        
        var client = new RestClient(apiUrl);

        var request = CreateDefaultRequest();
        request.AddStringBody(jsonString, ContentType.Json);
        
        var response = await client.ExecutePostAsync(request);

        Responses.Add(response);
        
        _scenarioContext["APIResponse"] = response;
        _resourceLocation = response.Headers!.FirstOrDefault(h => h.Name == "Location")?.Value?.ToString();

        return response;
    }
    
    private RestRequest CreateDefaultRequest()
    {
        var request = new RestRequest(ResourceLocation);

        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("X-User-ID", (string)_scenarioContext["API_USER_ID"]);
        request.AddHeader("X-Message-Hash", (string)_scenarioContext["X-Message-Hash"]);

        return request;
    }
}