using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Common.Specs.Helpers;

public class FundIns : FundInsRequest
{
    public string Datafile { get; set; }

    public string RecipientType { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
    
    //public List<fundingAccounts> FundingAccs { get; set; }
    public string NameOnAccount { get; set; }
}

public class fundingAccounts
{

}