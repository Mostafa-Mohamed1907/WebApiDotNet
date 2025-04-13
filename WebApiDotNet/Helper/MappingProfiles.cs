using AutoMapper;
using WebApiDotNet.Dtos;
using WebApiDotNet.Models;

namespace WebApiDotNet.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<RegisterDTO, ApplicationUser>()
                   .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));
            CreateMap<LoginDTO, ApplicationUser>();

        }

    }
}
