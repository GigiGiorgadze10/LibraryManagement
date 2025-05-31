using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Application.DTOs.AuthorDtos
{
    public class AuthorUpdateDto
    {
        [Required(ErrorMessage = "ID is required for update.")] 
        public int Id { get; set; }

        [Required(ErrorMessage = "Author full name is required.")]
        [StringLength(150, ErrorMessage = "Full name cannot exceed 150 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Author birth date is required.")]
        public System.DateTime BirthDate { get; set; }
    }
}