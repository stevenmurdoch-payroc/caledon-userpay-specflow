using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Assist;

namespace Common.Specs.ValueComparers;

/// <summary>
/// Overrides normal string comparison to accept the * wildcard.
/// Use by registering: Service.Instance.ValueComparers.Register(new StringWildcardComparer());
/// </summary>
public class StringWildcardComparer : IValueComparer
{
    public bool CanCompare(object actualValue)
    {
        return actualValue is string;
    }

    public bool Compare(string expectedValue, object actualValue)
    {
        var escapedExpectedValue = $"^{Regex.Escape(expectedValue).Replace("\\*", ".*").Replace("/", "\\/")}$";

        return Regex.IsMatch((string)actualValue, escapedExpectedValue);
    }
}