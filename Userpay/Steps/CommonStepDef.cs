using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using RestSharp;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using System.IO;
using System.Threading.Tasks;
using AccountBoardingAPI.Specs.Enums;
using PaymentAPI.Drivers;
using Common.Specs.Helpers;
using Common.Specs.Mappers;
using Common.Specs.ValueComparers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaymentAPI.Helpers;
using RestSharp.Serializers;

namespace PaymentAPI.Steps;

[Binding]
public sealed class CommonStepDef
{
    private readonly ScenarioContext _scenarioContext;
    private readonly RequestDriver _requestDriver;

    public CommonStepDef(
        ScenarioContext scenarioContext,
        RequestDriver requestDriver)
    {
        _scenarioContext = scenarioContext;
        _requestDriver = requestDriver;
        _scenarioContext["IsvId"] = ((int)IsvId.Standard).ToString();
    }

    [Given(@"the user is using the ""(.*)"" isv")]
    [Given(@"now the user is using the ""(.*)"" isv")]
    public void GivenTheUserIsUsingTheIsv(string isvIdString)
    {
        try
        {
            IsvId isvId = (IsvId) Enum.Parse(typeof(IsvId), isvIdString);
            _scenarioContext["IsvId"] = ((int) isvId).ToString();

            GivenTheUserHasAValidJwtToken();
        }
        catch(Exception ex)
        {
            throw new Exception($"'{isvIdString}' is not a configured ISV");
        }
    }
    
    [Given(@"the user has a valid jwt token")]
    public void GivenTheUserHasAValidJwtToken()
    {
        var isvId = (string)_scenarioContext["IsvId"];
        //_scenarioContext["Jwt"] = Jwt.GenerateToken(isvId, true);
    }

    [Given(@"the user has a fresh idempotency key")]
    public void GivenTheUserHasAFreshIdempotencyKey()
    {
        Guid idempotencyKey = Guid.NewGuid();
        _scenarioContext["idempotencyKey"] = idempotencyKey;
    }
    

    [Given(@"the user has a valid jwt token and unique idempotency key")]
    public void GivenTheUserHasAValidJwtAndUniqueIdempotencyKey()
    {
        GivenTheUserHasAValidJwtToken();
        GivenTheUserHasAFreshIdempotencyKey();
    }
    
    [Given(@"the user includes metadata")]
    [Then(@"the unencrypted metadata will be included")]
    [Given("the unencrypted metadata will be included")]
    public void GivenTheUserIncludesMetadata(Table table)
    {
        IDictionary<string, object> metadataConstruct = new ExpandoObject();
        var tableData = table.Header.Zip(table.Rows[0].Values);

        foreach (var item in tableData)
        {
            metadataConstruct[item.First] = item.Second;
        }

        _scenarioContext["metadata"] = metadataConstruct;
    }

    
    [Given(@"the user includes empty metadata")]
    public void GivenTheUserIncludesEmptyMetadata()
    {
        Object metadataConstruct = new Object();
        _scenarioContext["metadata"] = metadataConstruct;
    }

    [Then(@"the error response will be")]
    public void ThenTheErrorResponseWillBe(Table table)
    {
        //Register the wildcard comparer to allow * to be used in the search terms
        if (Service.Instance.ValueComparers.All(vc => vc.GetType() != typeof(StringWildcardComparer)))
            Service.Instance.ValueComparers.Register<StringWildcardComparer>();
        
        var response = (RestResponse)_scenarioContext["APIResponse"];
        //var problemDetails = JsonConvert.DeserializeObject<ProblemDetailsJSON>(response.Content);
            
        //table.CompareToInstance(problemDetails);
    }
    
    [Then(@"the list of errors will contain")]
    public void ThenTheListOfErrorsWillContain(Table table)
    {
        if (Service.Instance.ValueComparers.All(vc => vc.GetType() != typeof(StringWildcardComparer)))
            Service.Instance.ValueComparers.Register<StringWildcardComparer>();

        var response = (RestResponse)_scenarioContext["APIResponse"];
        var problemDetails = JsonConvert.DeserializeObject<ProblemDetailsJSON>(response.Content);
            
        table.CompareToSet(problemDetails.Errors);
    }

    [Given(@"the user has an invalid jwt token")]
    public void WhenGivenTheUserHasAnInvalidJwtToken()
    {
        _scenarioContext["Jwt"] = "any old rubbish";
    }
    
    [Given(@"the user prepares the ""(.*)"" payload")]
    [When(@"the user prepares the ""(.*)"" payload")]
    public void TheUserPreparesThePayload(string fileName)
    {
        // load file into scenarioContext["payload"] attribute
        string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/Data", fileName);
        
        JObject payload = JObject.Parse(File.ReadAllText(path));
        _scenarioContext["payload"] = payload;
    }
    
    [Given(@"the user prepares the Pad ""(.*)"" payload")]
    [When(@"the user prepares the Pad ""(.*)"" payload")]
    public void TheUserPreparesThePadPayload(string fileName)
    {
        // load file into scenarioContext["payload"] attribute
        string path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/Data", fileName);

        var payloadString = File.ReadAllText(path);
        var referenceNumber = GetRandomString();
        payloadString = payloadString.Replace("--referencenumber--", referenceNumber);
        if (_scenarioContext.ContainsKey("previousReferenceNumber"))
        {
            payloadString = payloadString.Replace("--previousreferencenumber--",
                (string) _scenarioContext["previousReferenceNumber"]);
        }

        JObject payload = JObject.Parse(payloadString);
        _scenarioContext["payload"] = payload;
        _scenarioContext["previousReferenceNumber"] = referenceNumber;
        
        
    }
    public string GetRandomString()
    {
        return DateTime.Now.ToString("yyyyMdhmmss");
    }

    [When(@"the user provides the additional headers")]
    public void TheUserProvidesTheHeaders(Table table)
    {
        var headersDict = table.MapToStringDictionary();
        _scenarioContext["additional-headers"] = headersDict;
    }
    
    //This is useful for testing that the API enforces required fields
    [Given(@"the user prepares an empty payload")]
    public void TheUserPreparesAnEmptyPayload()
    {
        _scenarioContext["payload"] = "{}";
    }

    [Given(@"the user adds the following FundingAccount attributes")]
    [When(@"the user adds the following FundingAccount attributes")]
    public void TheUserAddsTheFollowingFundAccAttributes(Table table)
    {
        var payload = (JObject) _scenarioContext["payload"];
        //var updateDataTable = table.CreateSet<FundingAccountJSON>().First();
        //var fundingPayLoad = JObject.Parse(JsonConvert.SerializeObject(updateDataTable));
        JArray data = payload.SelectToken("fundingAccounts") as JArray;
        //data.Add(fundingPayLoad);
        _scenarioContext["payload"] = payload;
    }
    
    [Given(@"the user updates the following attributes")]
    [When(@"the user updates the following attributes")]
    public void TheUserUpdatesTheFollowingAttributes(Table table)
    {
        var payload = (JObject) _scenarioContext["payload"];
        var updateDataTable = table.CreateSet<JSONDataTable>();

        foreach (var item in updateDataTable)
        {
            JToken data = payload.SelectToken(item.Path);
            data.Replace(item.Value);
        }

        _scenarioContext["payload"] = payload;
    }
    
    [Given(@"the '(.*)' is set to '(.*)'")]
    [When(@"the '(.*)' is modified to '(.*)'")]
    public void ThePropertyIsSetTo(string key, string value)
    {
        var payload = (JObject) _scenarioContext["payload"];

        JToken data = payload.SelectToken(key);
        data.Replace(value);
        
        _scenarioContext["payload"] = payload;
    }

    [When(@"the user sets the query string args to be")]
    public void WhenTheUserSetsTheQueryStringArgsToBe(Table table)
    {
        var tableDict = table.MapToStringDictionary();

        var toJoin = tableDict
            .Select(entry => $"{entry.Key}={entry.Value}");

        var args = String.Join('&', toJoin);

        _scenarioContext["paginationQueryString"] = args;
    }


    [When("a HEAD request is performed")]
    public async Task WhenAHeadRequestIsPerformed()
    {
        string apiUrl = _scenarioContext["API_URL"].ToString();
        var client = new RestClient(apiUrl);
        
        var request = RequestFactory.CreateDefault(_scenarioContext, _requestDriver);
        request.Method = Method.Head;
        var response = await client.ExecuteAsync(request);
        _scenarioContext["APIResponse"] = response;
    }

    [When("a PATCH request is performed")]
    public async Task WhenAPatchRequestIsPerformed()
    {
        string apiUrl = _scenarioContext["API_URL"].ToString();
        var client = new RestClient(apiUrl);
        
        var request = RequestFactory.CreateDefault(_scenarioContext, _requestDriver);
        request.Method = Method.Patch;
        var response = await client.ExecuteAsync(request);
        _scenarioContext["APIResponse"] = response;
    }

    
    [When("a GET request is performed")]
    public async Task WhenAGetRequestIsPerformed()
    {
        string apiUrl = _scenarioContext["API_URL"].ToString();
        var client = new RestClient(apiUrl);
        
        var request = RequestFactory.CreateDefault(_scenarioContext, _requestDriver);
        request.Method = Method.Get;
        var response = await client.ExecuteGetAsync(request);
        _scenarioContext["APIResponse"] = response;
    }
    
    [When(@"a delete request is performed")]
    public async Task WhenADeleteRequestIsPerformed()
    {
        string apiUrl = _scenarioContext["API_URL"].ToString();
        var client = new RestClient(apiUrl);
        
        var request = RequestFactory.CreateDefault(_scenarioContext, _requestDriver);
        request.Method = Method.Delete;
        var response = await client.ExecuteAsync(request);
        _scenarioContext["APIResponse"] = response;
    }
    
    [When(@"a POST request is performed with the custom headers")]
    public async Task WhenAPostRequestIsPerformedWithTheCustomHeaders()
    {
        string apiUrl = _scenarioContext["API_URL"].ToString();
        var client = new RestClient(apiUrl);
        var request = RequestFactory.CreateWithCustomHeaders(_scenarioContext, _requestDriver);

        string jsonString = _scenarioContext["payload"].ToString();
        request.AddStringBody(jsonString, ContentType.Json);
        
        var response = await client.ExecutePostAsync(request);
        _scenarioContext["APIResponse"] = response;

        HeaderParameter locationHeader = response.Headers!.FirstOrDefault(h => h.Name == "Location"); 
        if (locationHeader != null)
        {
            _requestDriver.ResourceLocation = locationHeader.Value!.ToString();    
        }
    }

    [When(@"an update is performed")]
    public async Task WhenAPutRequestIsPerformed()
    {
        string jsonString = _scenarioContext["payload"].ToString();
        string resourceLocation = _requestDriver.ResourceLocation;
        string apiUrl = _scenarioContext["API_URL"].ToString();

        if (!_scenarioContext.TryGetValue("ContentType", out string contentType))
        {
            contentType = "application/json";
        }
        
        if (!_scenarioContext.TryGetValue("Accept", out string accept))
        {
            accept = "application/json";
        }
        
        //creating the client - if the resourceLocation is the absolute, don't set the baseUrl
        var client = resourceLocation!.Contains(apiUrl!)
            ? new RestClient()
            : new RestClient(apiUrl);
        
        var request = new RestRequest(resourceLocation);
        
        request.AddStringBody(jsonString, ContentType.Json);
        
        request.AddHeader("Content-Type", contentType);
        request.AddHeader("Accept", accept);
        request.AddHeader("Authorization", "Bearer " + _scenarioContext["Jwt"]);
        request.AddHeader("Idempotency-Key", (Guid) _scenarioContext["idempotencyKey"]);
        
        var response = await client.ExecutePutAsync(request);
        
        _scenarioContext["APIResponse"] = response;
    }

    [Given(@"the request has a content-type of '(.*)'")]
    public void WhenTheRequestHasAContentTypeOfApplicationJson(string contentType)
    {
        _scenarioContext["ContentType"] = contentType;
        _scenarioContext["Accept"] = contentType;
        
    }
}