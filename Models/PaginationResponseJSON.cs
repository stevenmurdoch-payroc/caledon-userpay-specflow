using System.Collections.Generic;

namespace Common.Specs.Models;

public class PaginationResponseJSON<T>
{
    public int Limit { get; set; }
    public int Count { get; set; }
    public bool HasMore { get; set; }
    public IEnumerable<Link> Links { get; set; }
    public IEnumerable<T> Data { get; set; }
}