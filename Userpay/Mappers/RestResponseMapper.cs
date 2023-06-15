using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RestSharp;

namespace Common.Specs.Mappers;

public static class RestResponseMapper
{
    public static IDictionary<string, string> MapHeadersToStringDictionary(this RestResponse response)
    {
        var headerDict = response.Headers.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        return headerDict;
    }
}