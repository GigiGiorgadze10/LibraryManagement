// src/LibraryManagementSystem.Application/Mappings/MappingProfile.cs
using AutoMapper;
using LibraryManagement.Application.Contracts.Infrastructure;
using LibraryManagement.Application.DTOs.AuthorDtos;
using LibraryManagement.Application.DTOs.BookDtos;
using LibraryManagement.Application.DTOs.GenreDtos;
using LibraryManagement.Domain.Entities;
using System;

namespace LibraryManagement.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile(IDateTimeProvider dateTimeProvider)
        {
            CreateMap<AuthorCreateDto, Author>();
            CreateMap<AuthorUpdateDto, Author>();
            CreateMap<Author, AuthorReadDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.BirthDate, dateTimeProvider.UtcNow)));

            CreateMap<GenreCreateDto, Genre>();
            CreateMap<GenreUpdateDto, Genre>();
            CreateMap<Genre, GenreReadDto>();

            CreateMap<BookCreateDto, Book>();
            CreateMap<BookUpdateDto, Book>();
            CreateMap<Book, BookReadDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.FullName : string.Empty))
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre != null ? src.Genre.Name : string.Empty))
                .ForMember(dest => dest.BookAge, opt => opt.MapFrom(src => dateTimeProvider.UtcNow.Year - src.PublicationYear))
                .ForMember(dest => dest.IsThick, opt => opt.MapFrom(src => src.Pages > 100));
        }

        public MappingProfile() : this(new StaticDateTimeProvider()) 
        {
        }


        private static int CalculateAge(DateTime birthDate, DateTime currentDate)
        {
            var age = currentDate.Year - birthDate.Year;
            if (birthDate.Date > currentDate.AddYears(-age)) age--;
            return age;
        }

        private class StaticDateTimeProvider : IDateTimeProvider
        {
            public DateTime UtcNow => DateTime.UtcNow;
        }
    }
}