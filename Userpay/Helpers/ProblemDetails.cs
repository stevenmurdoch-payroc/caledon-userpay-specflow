using System.Collections.Generic;

namespace Common.Specs.Helpers;

public class ProblemDetailsJSON
{
    public string Type { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public string Detail { get; set; }
    public string Instance { get; set; }
    public string Resource { get; set; }
    public IEnumerable<ErrorJSON> Errors { get; set; }
}