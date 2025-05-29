// src/LibraryManagementSystem.Api/Providers/CustomJwtFormat.cs
// Ensure this class definition exists ONLY ONCE in this file.
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder; // For TextEncodings
using System;
using System.Configuration;
// Ensure these usings are correct for System.IdentityModel.Tokens.Jwt v6.x
using Microsoft.IdentityModel.Tokens; // For SymmetricSecurityKey, SigningCredentials, SecurityAlgorithms
using System.IdentityModel.Tokens.Jwt; // For JwtSecurityToken, JwtSecurityTokenHandler

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
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256); // Use from Microsoft.IdentityModel.Tokens

            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            var token = new JwtSecurityToken( // Use from System.IdentityModel.Tokens.Jwt
                _issuer,
                _audience,
                data.Identity.Claims,
                issued?.UtcDateTime, // Convert DateTimeOffset? to DateTime?
                expires?.UtcDateTime,
                signingCredentials);

            var handler = new JwtSecurityTokenHandler(); // Use from System.IdentityModel.Tokens.Jwt
            return handler.WriteToken(token);
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            // This ISecureDataFormat is used only for issuing (Protect method).
            // The validation of the token happens in the JwtBearerAuthenticationMiddleware.
            throw new NotImplementedException("Unprotect is not implemented for this JWT formatter as it's only used for token issuance.");
        }
    }
}