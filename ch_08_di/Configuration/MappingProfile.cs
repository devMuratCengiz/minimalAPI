using AutoMapper;
using ch_08_di.Entities.DTOs;

namespace ch_08_di.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDtoForInsertion>().ReverseMap();
            CreateMap<Book, BookDtoForUpdate>().ReverseMap();
        }
    }
}
