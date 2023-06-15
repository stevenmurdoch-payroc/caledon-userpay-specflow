using Newtonsoft.Json;
using RestSharp;
using TechTalk.SpecFlow;

namespace Common.Specs.Extensions;

public static class ScenarioContextExtensions
{
    public static RestResponse GetApiResponse(this ScenarioContext scenarioContext)
    {
        return (RestResponse)scenarioContext["APIResponse"];
    }
    
    public static string GetApiResponseContent(this ScenarioContext scenarioContext)
    {
        return ((RestResponse)scenarioContext["APIResponse"]).Content;
    }
    
    public static T GetApiResponseContentAs<T>(this ScenarioContext scenarioContext)
    {
        var content = ((RestResponse)scenarioContext["APIResponse"]).Content;
        return JsonConvert.DeserializeObject<T>(content);
    }
}