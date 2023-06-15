using System.Collections;
using Common.Specs.Helpers;
using Newtonsoft.Json;

namespace Common.Specs.Models;

public class FundingAccountResponseJSON
{
    public int FundingAccountId { get; set; }
    //TODO: Change this to datetime
    public string CreatedDate { get; set; }
    public string Status { get; set; }
    
    public string Type { get; set; }
    public string Use { get; set; }
    public string NameOnAccount { get; set; }
    //TODO: Payment methods
    //public IEnumerable<paymentMethods>
}