using System.Linq;
using PaymentAPI.Drivers;
using Common.Specs.Mappers;
using FluentAssertions;
using RestSharp;
using TechTalk.SpecFlow;

namespace PaymentAPI.Steps;

[Binding]
public sealed class ApiResponseStepDef
{
    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

    private readonly ScenarioContext _scenarioContext;
    private readonly ResponseValidationDriver _responseValidationDriver;

    public ApiResponseStepDef(
        ScenarioContext scenarioContext, 
        ResponseValidationDriver responseValidationDriver)
    {
        _scenarioContext = scenarioContext;
        _responseValidationDriver = responseValidationDriver;
    }
    
    [Then(@"the response code will be '(.*)'")]
    [Then(@"the response code will be response code '(.*)'")]
    [When(@"the response code will be response code '(.*)'")]
    public void ThenTheResponseCodeWillBeResponseCode(int checkStatus)
    {
        var response = (RestResponse) _scenarioContext["APIResponse"];
        int numericResponse = (int) response.StatusCode;
        numericResponse.Should().Be(checkStatus);
    }
    
    [Then(@"the response collection will contain")]
    public void ThenTheResponseCollectionWillContain(Table table)
    {
        _responseValidationDriver.ValidateThatTheApiResponseArrayContains(string.Empty, table);
    }
    
    [Then(@"the ""(.*)"" collection will contain")]
    public void ThenTheCollectionWillContain(string arrayName, Table table)
    {
        _responseValidationDriver.ValidateThatTheApiResponseArrayContains(arrayName, table);
    }
    
    [Then(@"the '(.*)' property will contain")]
    public void ThenThePropertyWillContain(string propertyName, Table table)
    {
        _responseValidationDriver.ValidateThatPropertyContains(propertyName, table);
    }

    [Then(@"the response will contain")]
    public void ThenTheResponseWillContain(Table table)
    {
        _responseValidationDriver.ValidateThatTheApiResponseContains(table);
    }
    
    // public void ThenTheResponseCodeWillBe(int checkStatus, Table table)
    //{
     //  var response = (RestResponse) _scenarioContext["APIResponse"];
     //int numericResponse = (int) response.StatusCode;
     //numericResponse.Should().Be(checkStatus);
    //}
    
    [Given(@"the request has a method call type of '(.*)'")]
    public void GivenTheRequestHasAMethodCallTypeOfGet(string methodCall)
    {
        //TODO:ScenarioContext.StepIsPending();
    }

    [When(@"the Idempotency Key (.*), URI (.*) and response body (.*) already exists")]
    public void WhenTheIdempotencyKeyUriAndResponseBodyAlreadyExists(string idempotencyKey, string uRI, string body)
    {
        //TODO:  ScenarioContext.StepIsPending();
    }

    [When(@"the Idempotency Key (.*) , URI (.*) and response body (.*) are unique")]
    public void WhenTheIdempotencyKeyUriAndResponseBodyAreUnique(string idempotencyKey, string uRI, string body)
    {
        //TODO:  ScenarioContext.StepIsPending();
    }
    
    [When(@"the response will have the headers with values")]
    public void WhenTheResponseWillHaveTheHeadersWithValues(Table table)
    {
        RestResponse response = (RestResponse) _scenarioContext["APIResponse"];
        var headerCases = table.MapToStringDictionary();
        var headerDict = response.Headers.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        
        foreach (var headerCase in headerCases)
        {
            var (headerName, expectedHeaderValue) = headerCase;
            var actualHeaderValue = headerDict[headerName];
            actualHeaderValue.Should().Match(expectedHeaderValue);
        }
    }

    [When(@"the request has a method of type")]
    public void WhenTheRequestHasAMethodOfType(Table table)
    {
     //TODO: ScenarioContext.StepIsPending();
    }

    [Then(@"the response message will display ""(.*)""")]
    public void ThenTheResponseMessageWillDisplay(string checkMessage)
    {
        var response = (RestResponse) _scenarioContext["APIResponse"];
        var stringResponse = response.Content;
        stringResponse.Should().Contain(checkMessage);
    }
}
