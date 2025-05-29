using LibraryManagement.Application.Contracts.Infrastructure;
using LibraryManagement.Infrastructure.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration; 
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text; 
using System.Threading.Tasks;
using Microsoft.Owin.Security.DataHandler.Encoder; 

namespace LibraryManagement.Infrastructure.Identity
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly AppUserManager _userManager;

        public JwtTokenGenerator(AppUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GenerateTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found for token generation.", nameof(userId));
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var userRoles = await _userManager.GetRolesAsync(user.Id);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretBytes = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["jwt:Secret"]);
            var key = new SymmetricSecurityKey(secretBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: ConfigurationManager.AppSettings["jwt:Issuer"],
                audience: ConfigurationManager.AppSettings["jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["jwt:DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}