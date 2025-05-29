// src/LibraryManagementSystem.Application/DTOs/BookDtos/BookFilterDto.cs
using LibraryManagement.Application.Common;
using LibraryManagement.Domain.Enums; // For SortDirection

namespace LibraryManagement.Application.DTOs.BookDtos
{
    public class BookFilterDto : PagedQueryBase
    {
        public int? MinPages { get; set; }
        public int? MaxPages { get; set; }
        public int? GenreId { get; set; }
        public int? AuthorId { get; set; }
        public string SortBy { get; set; } // e.g., "Title", "Pages", "PublicationYear"
        public SortDirection SortDirection { get; set; } = SortDirection.Asc;
    }
}