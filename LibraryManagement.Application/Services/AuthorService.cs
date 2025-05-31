using AutoMapper;
using LibraryManagement.Application.Contracts.Infrastructure;
using LibraryManagement.Application.DTOs.AuthorDtos;
using LibraryManagement.Application.Exceptions;
using LibraryManagement.Application.Services.Interfaces;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Interfaces;
using System; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AuthorService(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<AuthorReadDto> GetAuthorByIdAsync(int id)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(id);
            if (author == null)
                throw new NotFoundException(nameof(Author), id);
            return _mapper.Map<AuthorReadDto>(author);
        }

        public async Task<IEnumerable<AuthorReadDto>> GetAllAuthorsAsync()
        {
            var authors = await _unitOfWork.Authors.GetAllAsync();
            return _mapper.Map<IEnumerable<AuthorReadDto>>(authors);
        }

        public async Task<AuthorReadDto> CreateAuthorAsync(AuthorCreateDto authorCreateDto)
        {
            var age = _dateTimeProvider.UtcNow.Year - authorCreateDto.BirthDate.Year;
            if (authorCreateDto.BirthDate.Date > _dateTimeProvider.UtcNow.AddYears(-age)) age--; 

            if (age < 18)
            {
                throw new ValidationException("Author must be at least 18 years old.");
            }

            var author = _mapper.Map<Author>(authorCreateDto);
            await _unitOfWork.Authors.AddAsync(author);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<AuthorReadDto>(author); 
        }

        public async Task<bool> UpdateAuthorAsync(AuthorUpdateDto authorUpdateDto)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(authorUpdateDto.Id);
            if (author == null)
                throw new NotFoundException(nameof(Author), authorUpdateDto.Id);

            var age = _dateTimeProvider.UtcNow.Year - authorUpdateDto.BirthDate.Year;
            if (authorUpdateDto.BirthDate.Date > _dateTimeProvider.UtcNow.AddYears(-age)) age--;

            if (age < 18)
            {
                throw new ValidationException("Author must be at least 18 years old.");
            }

            _mapper.Map(authorUpdateDto, author); 
            _unitOfWork.Authors.Update(author); 
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var author = await _unitOfWork.Authors.GetByIdAsync(id);
            if (author == null)
                throw new NotFoundException(nameof(Author), id);

            _unitOfWork.Authors.Remove(author);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}