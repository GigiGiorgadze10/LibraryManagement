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
        // Constructor for DI if IDateTimeProvider is resolved for the profile itself
        public MappingProfile(IDateTimeProvider dateTimeProvider)
        {
            // Author Mappings
            CreateMap<AuthorCreateDto, Author>();
            CreateMap<AuthorUpdateDto, Author>();
            CreateMap<Author, AuthorReadDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.BirthDate, dateTimeProvider.UtcNow)));

            // Genre Mappings
            CreateMap<GenreCreateDto, Genre>();
            CreateMap<GenreUpdateDto, Genre>();
            CreateMap<Genre, GenreReadDto>();

            // Book Mappings
            CreateMap<BookCreateDto, Book>();
            CreateMap<BookUpdateDto, Book>();
            CreateMap<Book, BookReadDto>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.FullName : string.Empty))
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre != null ? src.Genre.Name : string.Empty))
                .ForMember(dest => dest.BookAge, opt => opt.MapFrom(src => dateTimeProvider.UtcNow.Year - src.PublicationYear))
                .ForMember(dest => dest.IsThick, opt => opt.MapFrom(src => src.Pages > 100));
        }

        // Parameterless constructor for AutoMapper to find it if DI isn't set up for profiles
        // or if IDateTimeProvider is injected into services that use IMapper instead.
        public MappingProfile() : this(new StaticDateTimeProvider()) // Fallback if IDateTimeProvider isn't injected.
        {
        }


        private static int CalculateAge(DateTime birthDate, DateTime currentDate)
        {
            var age = currentDate.Year - birthDate.Year;
            if (birthDate.Date > currentDate.AddYears(-age)) age--;
            return age;
        }

        // Helper class for parameterless constructor scenario
        private class StaticDateTimeProvider : IDateTimeProvider
        {
            public DateTime UtcNow => DateTime.UtcNow;
        }
    }
}