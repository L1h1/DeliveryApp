using AutoMapper;
using UserService.BLL.DTOs.Request;
using UserService.BLL.DTOs.Response;
using UserService.DAL.Models;

namespace UserService.BLL.Mapping
{
    internal class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterRequestDTO, User>();

            CreateMap<User, UserResponseDTO>()
                .ForMember(dest => dest.AccessToken, opt => opt.Ignore());

            CreateMap<User, UserDetailsDTO>();
        }
    }
}
