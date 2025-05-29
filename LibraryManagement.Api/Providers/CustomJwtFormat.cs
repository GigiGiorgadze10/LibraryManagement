using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder; 
using System;
using System.Configuration;
using Microsoft.IdentityModel.Tokens; 
using System.IdentityModel.Tokens.Jwt; 

namespace LibraryManagement.Api.Providers
{
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly byte[] _secret;


        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
            _audience = ConfigurationManager.AppSettings["jwt:Audience"];
            _secret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["jwt:Secret"]);
        }

        public string Protect(AuthenticationTicket data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var signingKey = new SymmetricSecurityKey(_secret);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256); 

            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            var token = new JwtSecurityToken( 
                _issuer,
                _audience,
                data.Identity.Claims,
                issued?.UtcDateTime, 
                expires?.UtcDateTime,
                signingCredentials);

            var handler = new JwtSecurityTokenHandler(); 
            return handler.WriteToken(token);
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException("Unprotect is not implemented for this JWT formatter as it's only used for token issuance.");
        }
    }
}