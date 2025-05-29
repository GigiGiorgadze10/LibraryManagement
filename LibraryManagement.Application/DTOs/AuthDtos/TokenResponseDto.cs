using System;
using System.Collections.Generic;

namespace LibraryManagement.Application.DTOs.AuthDtos
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; } 
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}