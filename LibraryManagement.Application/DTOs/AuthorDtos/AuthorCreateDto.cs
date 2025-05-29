// src/LibraryManagementSystem.Application/DTOs/AuthorDtos/AuthorCreateDto.cs
using System;
using System.ComponentModel.DataAnnotations; // For basic validation attributes

namespace LibraryManagement.Application.DTOs.AuthorDtos
{
    public class AuthorCreateDto
    {
        [Required(ErrorMessage = "Author full name is required.")]
        [StringLength(150, ErrorMessage = "Full name cannot exceed 150 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Author birth date is required.")]
        public DateTime BirthDate { get; set; }
    }
}