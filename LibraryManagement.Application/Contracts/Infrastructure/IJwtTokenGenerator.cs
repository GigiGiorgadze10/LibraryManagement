using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Contracts.Infrastructure
{
    public interface IJwtTokenGenerator
    {
        // Accepts userId (string) instead of AppUser to keep Application layer independent of Infrastructure concrete types
        Task<string> GenerateTokenAsync(string userId);
    }
}
