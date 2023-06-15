using System;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using RestSharp;
using FluentAssertions;


namespace PaymentAPI.Steps;

[Binding]
public class TransactionStepDef
{
    private readonly ScenarioContext _scenarioContext;

    public TransactionStepDef(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given(@"the user attempts to connect to the Payment API")]
    public void GivenTheUserAttemptsToConnectToThePaymentApi()
    {

        _scenarioContext["ApiUrl"] = Environment.GetEnvironmentVariable("API_URL");
    }

    [Then(@"the response code will be '(.*)'")]
    public void ThenTheResponseCodeWillBe(int checkStatus, Table table)
    {
        var response = (RestResponse) _scenarioContext["APIResponse"];
        int numericResponse = (int) response.StatusCode;
        numericResponse.Should().Be(checkStatus);
    }
}