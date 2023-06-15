using System;
using System.Collections.Generic;

namespace Common.Specs.Helpers;

public class FundingAccountJSON
{
    public FundingAccountJSON()
    {
        paymentMethods = new List<paymentMethods>();
    }
    public string type { get; set;}
    public string use { get; set;}
    public string nameOnAccount { get; set; }
    public List <paymentMethods> paymentMethods { get; set; } 
    //public IEnumerable<PaymentMethodJSON> PaymentMethods { get; set;}
}

public class paymentMethods
{
    public string type { get; set; } 
    public string routingNumber { get; set; }
    public string accountNumber { get; set; }
}