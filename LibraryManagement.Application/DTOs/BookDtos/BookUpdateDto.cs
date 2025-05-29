using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Application.DTOs.BookDtos
{
    public class BookUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Book title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Author ID must be a positive integer.")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Genre ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Genre ID must be a positive integer.")]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Number of pages is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Pages must be a positive integer.")]
        public int Pages { get; set; }

        [Required(ErrorMessage = "Publication year is required.")]
        public int PublicationYear { get; set; }
    }
}