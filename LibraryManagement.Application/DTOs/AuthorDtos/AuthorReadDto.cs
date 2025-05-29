// src/LibraryManagementSystem.Application/DTOs/AuthorDtos/AuthorReadDto.cs
using System;

namespace LibraryManagement.Application.DTOs.AuthorDtos
{
    public class AuthorReadDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; } // Calculated property
    }
}