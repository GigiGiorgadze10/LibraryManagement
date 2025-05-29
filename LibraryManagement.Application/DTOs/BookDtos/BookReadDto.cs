// src/LibraryManagementSystem.Application/DTOs/BookDtos/BookReadDto.cs
namespace LibraryManagement.Application.DTOs.BookDtos
{
    public class BookReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; } // Mapped from Author.FullName
        public string GenreName { get; set; }  // Mapped from Genre.Name
        public int Pages { get; set; }
        public int PublicationYear { get; set; }
        public int BookAge { get; set; }      // Calculated: CurrentYear - PublicationYear
        public bool IsThick { get; set; }      // Calculated: Pages > 100
    }
}