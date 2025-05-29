using LibraryManagement.Application.DTOs.AuthorDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<AuthorReadDto> GetAuthorByIdAsync(int id);
        Task<IEnumerable<AuthorReadDto>> GetAllAuthorsAsync();
        Task<AuthorReadDto> CreateAuthorAsync(AuthorCreateDto authorCreateDto);
        Task<bool> UpdateAuthorAsync(AuthorUpdateDto authorUpdateDto);
        Task<bool> DeleteAuthorAsync(int id);
    }
}