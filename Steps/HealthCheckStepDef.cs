using System;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using RestSharp;
using FluentAssertions;


namespace PaymentAPI.Steps;

[Binding]
public class HealthCheckStepDef
{
    private readonly ScenarioContext _scenarioContext;

    public HealthCheckStepDef(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }
    
    [Given(@"the user attempts to connect to the Universal API")]
    public void GivenTheUserAttemptsToConnectToTheUniversalApi()
    {
        
        _scenarioContext["ApiUrl"] = Environment.GetEnvironmentVariable("API_URL");
    }

    [When(@"the request hits the Universal API Health Check")]
    public async Task WhenTheRequestHitsTheUniversalApiHealthCheck()
    {
        var url = (string) _scenarioContext["ApiUrl"];
        var client = new RestClient(url);
        var request = new RestRequest("healthcheck");
        var response = await client.GetAsync(request);
        
        _scenarioContext["APIResponse"] = response;

        DateTime responseTimeStamp = new DateTime();
        responseTimeStamp = DateTime.Now;

        _scenarioContext["responseTimeStamp"] = responseTimeStamp;
    }

    [Then(@"current version of the Universal API")]
    public void ThenCurrentVersionOfTheUniversalApi()
    {
        //TODO: Work out how we infer the expected version
    }
   
    [Then(@"the health check response code will be '(.*)'")]
    public void ThenTheHealthCheckResponseCodeWillBe(int checkStatus, Table table)
    {
        var response = (RestResponse) _scenarioContext["APIResponse"];
        int numericResponse = (int) response.StatusCode;
        numericResponse.Should().Be(checkStatus);
    }

    [When(@"the Universal API is down")]
    public void WhenTheUniversalApiIsDown()
    {
        //TODO:  ScenarioContext.StepIsPending();
    }

    [Then(@"the request times out")]
    public void ThenTheRequestTimesOut()
    {
        //TODO:  ScenarioContext.StepIsPending();
    }
}