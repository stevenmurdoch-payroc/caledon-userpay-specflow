using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Gherkin.Ast;
using RestSharp;
using RestSharp.Serializers;
using TechTalk.SpecFlow;

namespace Common.Specs.Helpers;

public static class Seeding
{
    public static Task<RestResponse> Seed(ScenarioContext context, string endpoint)
    {
        Guid idempotencyKey = Guid.NewGuid();
        string jsonString = context["payload"].ToString();
        
        //creating the JSON payload and settings
        var client = new RestClient(context["API_URL"].ToString());
        var request = new RestRequest(endpoint);
        
        request.AddStringBody(jsonString, ContentType.Json);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", "Bearer " + context["Jwt"]);
        request.AddHeader("Idempotency-Key", idempotencyKey);
        
        //We're only expecting 201's, anything else should be reported
        var responseTask = client.ExecutePostAsync(request);
        return responseTask;
    }

    //Post a payload to an endpoint, for the purpose of seeding the database for subsequent tests
    public static async Task Seed(ScenarioContext context, int times, string endpoint)
    {
        IEnumerable<Task<RestResponse>> responseTaskCollection = Enumerable
            .Range(0, times)
            .Select(_ => Seed(context, endpoint))
            .ToList();

        var allResponses = await Task.WhenAll(responseTaskCollection);

        var failedResponses = allResponses.Where(response => response.StatusCode != HttpStatusCode.Created);

        if (failedResponses.Any())
        {
            var failingTestsCount = failedResponses.Count();
            var failingTestMessage = failedResponses.First().Content;
            throw new Exception($"There was a failure during seeding - {failingTestsCount} failed with the message - {failingTestMessage}");
        }
    }
}