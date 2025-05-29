// src/LibraryManagementSystem.Application/DTOs/AuthDtos/TokenResponseDto.cs
using System;
using System.Collections.Generic;

namespace LibraryManagement.Application.DTOs.AuthDtos
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; } // Consider using a string representation like ISO 8601 if preferred for client
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}