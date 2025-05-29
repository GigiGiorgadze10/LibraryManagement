// src/LibraryManagementSystem.Application/Services/Interfaces/IBookService.cs
using LibraryManagement.Application.DTOs.BookDtos;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Services.Interfaces
{
    public interface IBookService
    {
        Task<BookReadDto> GetBookByIdAsync(int id);
        Task<PaginatedBookResultDto> GetBooksAsync(BookFilterDto filter);
        Task<BookReadDto> CreateBookAsync(BookCreateDto bookCreateDto);
        Task<bool> UpdateBookAsync(BookUpdateDto bookUpdateDto);
        Task<bool> DeleteBookAsync(int id);
    }
}