// src/LibraryManagementSystem.Application/DTOs/BookDtos/PaginatedBookResultDto.cs
using LibraryManagement.Application.DTOs.BookDtos;
using System.Collections.Generic;

namespace LibraryManagement.Application.DTOs.BookDtos
{
    public class PaginatedBookResultDto
    {
        public IEnumerable<BookReadDto> Items { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public PaginatedBookResultDto()
        {
            Items = new List<BookReadDto>();
        }
    }
}