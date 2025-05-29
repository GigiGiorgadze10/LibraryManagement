// src/LibraryManagement.Api/Controllers/AuthController.cs
using LibraryManagement.Application.Contracts.Infrastructure; // For IJwtTokenGenerator
using LibraryManagement.Application.DTOs.AuthDtos;
using LibraryManagement.Infrastructure.Identity; // For AppUser, AppUserManager, Roles
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin; // For GetOwinContext() and GetUserManager<>()
using System; // For Convert, DateTime
using System.Collections.Generic; // For IList
using System.Configuration; // For ConfigurationManager
using System.Net.Http; // For Request
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace LibraryManagement.Api.Controllers
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        // Property to safely access UserManager, resolving from OWIN context.
        // This relies on AppUserManager being correctly registered with app.CreatePerOwinContext in Startup.cs
        private AppUserManager UserManager => Request.GetOwinContext().GetUserManager<AppUserManager>();

        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthController(IJwtTokenGenerator jwtTokenGenerator) // Autofac injects IJwtTokenGenerator
        {
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Register(UserRegistrationDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser() { UserName = model.Email, Email = model.Email };
            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            await UserManager.AddToRoleAsync(user.Id, Roles.User);
            return Ok("User registered successfully.");
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ResponseType(typeof(TokenResponseDto))]
        public async Task<IHttpActionResult> Login(UserLoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AppUser user = await UserManager.FindAsync(model.Email, model.Password);

            if (user == null)
            {
                return Unauthorized(); // Or BadRequest("Invalid email or password.")
            }

            var tokenString = await _jwtTokenGenerator.GenerateTokenAsync(user.Id);
            var userRoles = await UserManager.GetRolesAsync(user.Id);

            var tokenResponse = new TokenResponseDto
            {
                Token = tokenString,
                Email = user.Email,
                Roles = userRoles, // Directly assign IList<string>
                Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["jwt:DurationInMinutes"]))
            };

            return Ok(tokenResponse);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid) // This means no errors were added to model state despite result not succeeding
                {
                    // Return a more generic error if IdentityResult failed but didn't populate ModelState
                    return BadRequest("User operation failed. Please check the details.");
                }
                return BadRequest(ModelState);
            }
            return null; // Should not be reached if result.Succeeded is false and errors are handled.
                         // Can indicate an issue if it is.
        }
    }
}