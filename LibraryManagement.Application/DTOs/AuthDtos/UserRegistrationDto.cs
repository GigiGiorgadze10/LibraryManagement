// src/LibraryManagementSystem.Application/DTOs/AuthDtos/UserRegistrationDto.cs
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Application.DTOs.AuthDtos
{
    public class UserRegistrationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}