// src/LibraryManagementSystem.Application/Services/Interfaces/IGenreService.cs
using LibraryManagement.Application.DTOs.GenreDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Services.Interfaces
{
    public interface IGenreService
    {
        Task<GenreReadDto> GetGenreByIdAsync(int id);
        Task<IEnumerable<GenreReadDto>> GetAllGenresAsync();
        Task<GenreReadDto> CreateGenreAsync(GenreCreateDto genreCreateDto);
        Task<bool> UpdateGenreAsync(GenreUpdateDto genreUpdateDto); // ✅ ADD THIS
        Task<bool> DeleteGenreAsync(int id);
    }
}