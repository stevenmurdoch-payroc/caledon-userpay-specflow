using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using RestSharp;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using System.Threading.Tasks;
using Common.Specs.Extensions;
using Common.Specs.Mappers;
using Common.Specs.Models;
using Newtonsoft.Json.Linq;

namespace PaymentAPI.Steps;

[Binding]
public sealed class HalStepDef
{
    private readonly ScenarioContext _scenarioContext;
    private readonly RestClient _client;

    public HalStepDef(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _client = new RestClient(_scenarioContext["API_URL"].ToString());
    }
    
    [Then(@"the links will contain")]
    public void ThenTheResponseWillContainTheLinks(Table table)
    {
        var actualLinks = _scenarioContext["Links"] as IEnumerable<Link>;   
        table.CompareToSet<Link>(actualLinks);
    }
    
    [When(@"i examine the links at the path (.*)")]
    public void WhenIExamineTheLinksAtThePath(string path)
    {
        var response = (RestResponse) _scenarioContext["APIResponse"];
        var responseJObject = JObject.Parse(response.Content);
        JToken linksToken = responseJObject.SelectToken(path);

        IEnumerable<Link> linksList;
        
        if (linksToken.Type == JTokenType.Array)
        {
            linksList = linksToken.Select(link => link.ToObject<Link>());
        }
        else if (linksToken.Type == JTokenType.Object)
        {
            linksList = new List<Link>() {linksToken.ToObject<Link>()};
        }
        else if (linksToken.Type == JTokenType.Null)
        {
            throw new Exception($"The JSpath {path} does not exist in the response");
        }
        else
        {
            throw new Exception($"Links path {path} points to an unsupported JToken type.");
        }
        
        _scenarioContext["Links"] = linksList;
    }
    
    [When(@"i follow the link '(.*)'")]
    public async Task ThenIFollowTheLink(string linkRel)
    {
        //Get the list of links
        var links = (IEnumerable<Link>) _scenarioContext["Links"];
        
        //Find the one with rel 'next'
        var linkObject = links.First(x => x.Rel == linkRel);
        var url = linkObject.Href;

        //Follow it
        var request = new RestRequest(url);
        
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", "Bearer " + _scenarioContext["Jwt"]);

        var response = await _client.ExecuteAsync(request);
        _scenarioContext["APIResponse"] = response;
    }

    [When(@"the user recursively follows the link at path ""(.*)"" with rel ""(.*)""")]
    public async Task WhenTheUserRecursivelyFollowsTheLinkAtPathWithRel(string path, string rel)
    {
        try
        {
            while (true)
            {
                WhenIExamineTheLinksAtThePath(path);
                //This should throw an exception, when we get to the end and the link doesn't exist anymore
                await ThenIFollowTheLink(rel);
            }
        }
        catch (Exception ex)
        {
            
        }
    }
    
   // [When(@"the user recursively searches until they find ""(.*)"" with a value matching the stored ""(.*)""")]
    //public async Task WhenTheUserRecursivleySearchesUntilTheyFindWithAValueOf(string dataKey, string key)
    //{
      //  var targetValue = (string) _scenarioContext[key];
        
        //while (true)
        //{
          //  var response = (RestResponse) _scenarioContext["APIResponse"];
            //var responseJObject = JObject.Parse(response.Content);
            //JToken dataToken = responseJObject.SelectToken("$.data");

            //var target = dataToken.FirstOrDefault(recipient => recipient.Value<string>(dataKey) == targetValue, null);

            //if (target != null)
            //{
              //  _scenarioContext["Found Recipient"] = target.ToString();
              //  break;
            //}

           //WhenIExamineTheLinksAtThePath("$.links");
            
           // try
           // {
           //     await ThenIFollowTheLink("next");
           // }
           // catch (Exception ex)
          //  {
          //      throw new Exception($"Ran out of pages before key {dataKey} with value {targetValue} could be found.");
          //  }
      //  }
  //  }

    [Then("the response will contain the pagination details")]
    public void ThenTheResponseWillContainThePaginatedDetails(Table table)
    {
        var actualValues = _scenarioContext.GetApiResponseContentAs<PaginationResponseJSON<dynamic>>();
        table.CompareToInstance(actualValues);
    }
}