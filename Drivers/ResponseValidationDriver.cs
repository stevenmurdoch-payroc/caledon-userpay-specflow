using System;
using System.Linq;
using System.Text.Json;
//using System.Text.Json.Nodes;
using FluentAssertions;
using RestSharp;
using TechTalk.SpecFlow;

namespace PaymentAPI.Drivers
{
    public class ResponseValidationDriver
    {
        private readonly ScenarioContext _scenarioContext;

        public ResponseValidationDriver(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        public void ValidateThatPropertyContains(string propertyName, Table table)
        {
            foreach (var header in table.Header)
            {
                table.RenameColumn(header, $"{propertyName}.{header}");
            }
            
            ValidateThatTheApiResponseContains(table);
        }
        
        public void ValidateThatTheApiResponseContains(Table table)
        {
            var response = (RestResponse) _scenarioContext["APIResponse"];
            var responseContentJson = JsonDocument.Parse(response.Content);
            var sequence = table.Header.Zip(table.Rows[0].Values);

            foreach (var (key, expectedValue) in sequence)
            {
                var element = responseContentJson.RootElement;
                var splitKey = key.Split('.');

                try
                {
                    element = splitKey
                        .Aggregate(element, (current, splitValue) => 
                            current.GetProperty(splitValue));
                }
                catch (Exception)
                {
                    throw new Exception($"Key '{key}' was not found in the response body");
                }

                string elementAsString = element.ToString();

                JsonValueKind[] lowerCaseScenarios = {JsonValueKind.True, JsonValueKind.False}; 
                if(lowerCaseScenarios.Contains(element.ValueKind))
                {
                    elementAsString = elementAsString.ToLower();
                }
                
                elementAsString.Should().Match(expectedValue);
            }
        }
        
        public void ValidateThatTheApiResponseArrayContains(string arrayName, Table table)
        {
            var response = (RestResponse) _scenarioContext["APIResponse"];
            var responseContentJson = JsonDocument.Parse(response.Content);
            var sequence = table.Header.Zip(table.Rows[0].Values);

            var baseArray = responseContentJson.RootElement;

            if (!string.IsNullOrEmpty(arrayName))
            {
                var arraySplit = arrayName.Split('.');
            
                baseArray = arraySplit
                    .Aggregate(baseArray, (current, splitValue) => 
                        current.GetProperty(splitValue));
            }

            foreach (var (key, expectedValue) in sequence)
            {
                bool found = false;
                var element = baseArray;
                
                foreach (var arrayElement in baseArray.EnumerateArray())
                {
                    element = arrayElement;
                    
                    var splitKey = key.Split('.');

                    foreach (var s in splitKey)
                    {
                        if (element.TryGetProperty(s, out element) && 
                            element.ToString() == expectedValue)
                        {
                            found = true;
                        }
                    }

                    if (found)
                        break;
                }
                
                element.ToString().Should().Match(expectedValue);
            }
        }
    }
}