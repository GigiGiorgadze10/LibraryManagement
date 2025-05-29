using AutoMapper;
using LibraryManagement.Application.DTOs.GenreDtos;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Services.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Services
{
    public class GenreService : IGenreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GenreService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GenreReadDto> GetGenreByIdAsync(int id)
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync(id);
            if (genre == null)
                throw new NotFoundException(nameof(Genre), id);
            return _mapper.Map<GenreReadDto>(genre);
        }

        public async Task<IEnumerable<GenreReadDto>> GetAllGenresAsync()
        {
            var genres = await _unitOfWork.Genres.GetAllAsync();
            return _mapper.Map<IEnumerable<GenreReadDto>>(genres);
        }

        public async Task<GenreReadDto> CreateGenreAsync(GenreCreateDto genreCreateDto)
        {
            var existingGenreByName = (await _unitOfWork.Genres.FindAsync(g => g.Name.ToLower() == genreCreateDto.Name.ToLower())).FirstOrDefault();
            if (existingGenreByName != null)
            {
                throw new ValidationException($"Genre with name '{genreCreateDto.Name}' already exists.");
            }

            var genre = _mapper.Map<Genre>(genreCreateDto);
            await _unitOfWork.Genres.AddAsync(genre);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<GenreReadDto>(genre);
        }

        // ✅ ADDED UpdateGenreAsync
        public async Task<bool> UpdateGenreAsync(GenreUpdateDto genreUpdateDto)
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync(genreUpdateDto.Id);
            if (genre == null)
                throw new NotFoundException(nameof(Genre), genreUpdateDto.Id);

            // Check if another genre with the new name already exists (excluding the current one)
            var existingGenreByName = (await _unitOfWork.Genres.FindAsync(g => g.Id != genreUpdateDto.Id && g.Name.ToLower() == genreUpdateDto.Name.ToLower())).FirstOrDefault();
            if (existingGenreByName != null)
            {
                throw new ValidationException($"Another genre with the name '{genreUpdateDto.Name}' already exists.");
            }

            _mapper.Map(genreUpdateDto, genre); // Update properties from DTO to entity
            _unitOfWork.Genres.Update(genre);    // Mark entity as modified
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // ✅ ADDED DeleteGenreAsync
        public async Task<bool> DeleteGenreAsync(int id)
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync(id);
            if (genre == null)
                throw new NotFoundException(nameof(Genre), id);

            // Optional: Check if genre is associated with any books before deletion
            // var booksWithGenre = await _unitOfWork.Books.FindAsync(b => b.GenreId == id);
            // if (booksWithGenre.Any())
            // {
            //     throw new ValidationException("Cannot delete genre. It is currently associated with one or more books.");
            // }

            _unitOfWork.Genres.Remove(genre);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}