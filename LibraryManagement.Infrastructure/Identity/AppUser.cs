using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
namespace LibraryManagement.Infrastructure.Identity
{
    public class AppUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager, string authenticationType)
        { 
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType); return userIdentity; 
        }
    }
}