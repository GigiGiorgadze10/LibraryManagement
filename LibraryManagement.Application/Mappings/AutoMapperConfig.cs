// src/LibraryManagementSystem.Application/Mappings/AutoMapperConfig.cs
using AutoMapper;
using LibraryManagement.Application.Contracts.Infrastructure;

namespace LibraryManagement.Application.Mappings
{
    public static class AutoMapperConfig
    {
        public static IMapper Initialize(IDateTimeProvider dateTimeProvider)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                // Pass the dateTimeProvider to the profile constructor
                cfg.AddProfile(new MappingProfile(dateTimeProvider));
                // Add other profiles here if you have more
            });
            return mapperConfiguration.CreateMapper();
        }

        // Overload for simpler setup if IDateTimeProvider injection to profile is complex
        // and the profile's parameterless constructor handles the fallback.
        public static IMapper Initialize()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>(); // Uses parameterless constructor of MappingProfile
            });
            return mapperConfiguration.CreateMapper();
        }
    }
}