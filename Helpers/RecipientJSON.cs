using System.Collections;

namespace Common.Specs.Helpers;

public class RecipientJSON
{
    public string RecipientId { get; set;}

    public string Status { get; set;}

    public string RecipientType { get; set;}

    public string TaxId { get; set;}

    public string CharityId { get; set;}
    
    public string DoingBusinessAs { get; set;}
    
    public AddressJSON Address { get; set;}
    
    //public string ContactMethods { get; set;}
    
    //public string Metadata { get; set;}
}