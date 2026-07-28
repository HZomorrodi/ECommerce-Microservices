using AutoMapper;
using eCommerce.API.Controllers;
using eCommerce.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Core.Mappers
{
    public class ApplicationUserToUserDTOMappingProfile : Profile
    {
        public ApplicationUserToUserDTOMappingProfile()
        {
            CreateMap<ApplicationUser, UserDTO>()
              .ForMember(des => des.UserID, opt => opt.MapFrom(src => src.UserId))
              .ForMember(des => des.Email, opt => opt.MapFrom(src => src.Email))
              .ForMember(des => des.PersonName, opt => opt.MapFrom(src => src.PersonName))
              .ForMember(des => des.Gender, opt => opt.MapFrom(src => src.Gender));
        }
    }
}
