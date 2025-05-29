// src/LibraryManagementSystem.Api/Providers/CustomOAuthProvider.cs
using Autofac; // For IContainer if resolving through it, though GetUserManager is preferred
using LibraryManagement.Infrastructure.Identity; // For AppUser, AppUserManager
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin; // For context.GetUserManager<AppUserManager>()
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibraryManagement.Api.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly Autofac.IContainer _container; // ✅ Fully qualify // Keep if you need to resolve other services

        public CustomOAuthProvider(Autofac.IContainer container) // ✅ Fully qualify
        {
            _container = container;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // Allow CORS for the token endpoint if needed from different origins (optional)
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            // Resolve UserManager from OWIN context per request - this is generally preferred
            var userManager = context.OwinContext.GetUserManager<AppUserManager>();
            // Alternative if GetUserManager isn't working due to DI setup:
            // var userManager = _container.Resolve<AppUserManager>(); (Ensure correct lifetime scope if using this)

            try
            {
                AppUser user = await userManager.FindAsync(context.UserName, context.Password);

                if (user == null)
                {
                    context.SetError("invalid_grant", "Invalid username or password.");
                    return;
                }

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
                // Add roles to claims
                var roles = await userManager.GetRolesAsync(user.Id); // user.Id is inherited
                foreach (var role in roles)
                {
                    oAuthIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                // You can add more custom claims to oAuthIdentity here if needed
                // For example: oAuthIdentity.AddClaim(new Claim("CustomClaim", "CustomValue"));

                var ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
                context.Validated(ticket);
            }
            catch (System.Exception ex)
            {
                // Log error (e.g., using System.Diagnostics.Trace or a logging library)
                System.Diagnostics.Trace.TraceError("Error in GrantResourceOwnerCredentials: " + ex.ToString());
                context.SetError("server_error", "An unexpected error occurred.");
            }
            // UserManager from OWIN context is typically managed by the context for disposal.
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // For Resource Owner Password Credentials Grant, client authentication is not typically required.
            // If you have client_id/client_secret, validate them here.
            // For this example, we assume public clients or no client validation for simplicity.
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            // Add custom parameters to the token response if needed
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }
    }
}