// src/LibraryManagementSystem.Application/Services/GenreService.cs
using AutoMapper;
using LibraryManagement.Application.DTOs.GenreDtos;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Services.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq; // For FirstOrDefault
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
            // Example: Check if genre with the same name already exists
            var existingGenre = (await _unitOfWork.Genres.FindAsync(g => g.Name.ToLower() == genreCreateDto.Name.ToLower())).FirstOrDefault();
            if (existingGenre != null)
            {
                throw new ValidationException($"Genre with name '{genreCreateDto.Name}' already exists.");
            }

            var genre = _mapper.Map<Genre>(genreCreateDto);
            await _unitOfWork.Genres.AddAsync(genre);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<GenreReadDto>(genre);
        }
    }
}