using System;
using PaymentAPI.Drivers;
using System.Collections.Generic;
using RestSharp;
using TechTalk.SpecFlow;

namespace PaymentAPI.Helpers;

public static class RequestFactory
{
    public static RestRequest CreateDefault(ScenarioContext context, RequestDriver requestDriver)
    {
        //Get the url from the context
        string resourceLocation = requestDriver.ResourceLocation;
        
        var request = new RestRequest(resourceLocation);

        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Authorization", $"Bearer {context["Jwt"]}");
        request.AddHeader("Idempotency-Key", (Guid)context["idempotencyKey"]);

        return request;
    }
    
    public static RestRequest CreateWithCustomHeaders(ScenarioContext context, RequestDriver requestDriver)
    {
        var request = CreateDefault(context, requestDriver);
        
        var headerKeys = new Dictionary<string, string>()
        {
            {"ContentType", "Content-Type"},
            {"Accept", "Accept"},
        };

        foreach (var pair in headerKeys)
        {
            var (contextKey, headerName) = pair;
            
            if (context.ContainsKey(contextKey))
            {
                request.AddHeader(headerName, (string)context[contextKey]);
            } 
        }

        return request;
    }
}