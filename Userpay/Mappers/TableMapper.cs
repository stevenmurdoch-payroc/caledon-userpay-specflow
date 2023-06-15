using System;
using System.Collections;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using System.Linq;

namespace Common.Specs.Mappers;

public static class TableMapper
{
    public static IDictionary<string, string> MapToStringDictionary(this Table table)
    {
        if (table.RowCount > 1)
        {
            throw new Exception("Tables being used as dictionaries, can only contain one row.");
        }
        
        var dictionary = new Dictionary<string, string>();
        var tableData = table.Header.Zip(table.Rows[0].Values);

        foreach (var entry in tableData)
        {
            var (header, rowValue) = entry;
            dictionary[header] = rowValue;
        }
        
        return dictionary;
    }
    
    public static IEnumerable<IDictionary<string, string>> MapToStringDictionarys(this Table table)
    {
        var dictionaries = new List<IDictionary<string, string>>();
        
        foreach (var row in table.Rows)
        {
            var dictionary = new Dictionary<string, string>();
            var tableData = table.Header.Zip(row.Values);
            foreach (var entry in tableData)
            {
                var (header, rowValue) = entry;
                dictionary[header] = rowValue;
            }
            dictionaries.Add(dictionary);
        }
        
        return dictionaries;
    }
}