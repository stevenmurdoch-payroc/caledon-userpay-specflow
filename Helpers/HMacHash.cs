using System.Security.Cryptography;

namespace PaymentAPI.Helpers;

public class HMacHash
{
    private byte[] Key { get; }
    private byte[] Message { get; }
    
    public HMacHash(string key, string message)
    {
        Key = StringEncode(key);
        Message = StringEncode(message);
    }
    
    public string ComputeHash()
    {
        var hmac = new HMACSHA256(Key);
        
        var hashedBytes = hmac.ComputeHash(Message);
            
        var base64String = Convert.ToBase64String(hashedBytes);

        return base64String.Replace("=", "");
    }
    
    private static byte[] StringEncode(string text)
    {
        var encoding = new System.Text.ASCIIEncoding();
        return encoding.GetBytes(text);
    }
    
}