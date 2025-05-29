// src/LibraryManagementSystem.Application/DTOs/GenreDtos/GenreCreateDto.cs
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Application.DTOs.GenreDtos
{
    public class GenreCreateDto
    {
        [Required(ErrorMessage = "Genre name is required.")]
        [StringLength(100, ErrorMessage = "Genre name cannot exceed 100 characters.")]
        public string Name { get; set; }
    }
}