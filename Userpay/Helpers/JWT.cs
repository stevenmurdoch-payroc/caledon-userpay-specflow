using System;
using System.ComponentModel.Design;
using System.Text.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Google.Protobuf.WellKnownTypes;
using Io.Cucumber.Messages;
using Microsoft.IdentityModel.Tokens;

namespace PaymentAPI.Specs.Helpers;

public static class Jwt
{
    public static string GenerateToken(string isvId, bool valid)
    {
        byte[] randomSecret = new byte[16];
        var rand = new Random();
        rand.NextBytes(randomSecret);

        var mySecurityKey = new SymmetricSecurityKey(randomSecret);
        var myIssuer = "https://api.payroc.com";
        var myAudience = "https://api.payroc.com";

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, isvId),
                new Claim("account_type", "isg")
            }),
            //Expires = DateTime.UtcNow.AddDays(7),
            Issuer = myIssuer,
            Audience = myAudience,
            SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);            
    }
}



/*   public static string GenerateInvalidToken(string isvId)
  {
      var token = "9999999";
      return token;
      //var token = GenerateToken(isvId);
      //return token.Trim('.');
  }
/* 
{
  public static int GenerateInvalidTypeToken(string isvId)
  {

      return 1;

  }

  /* public static string GenerateTokenForNonExistentISV(string isvId)
   {

       var token = GenerateToken(isvId);
       // set to int max to make a token that doesnt exist
       return token

   }*/