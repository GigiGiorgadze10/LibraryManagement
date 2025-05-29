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
                cfg.AddProfile(new MappingProfile(dateTimeProvider));
         
            });
            return mapperConfiguration.CreateMapper();
        }

        public static IMapper Initialize()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            return mapperConfiguration.CreateMapper();
        }
    }
}