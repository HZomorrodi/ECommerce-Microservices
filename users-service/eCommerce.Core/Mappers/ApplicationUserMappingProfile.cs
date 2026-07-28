using AutoMapper;
using eCommerce.Core.DTO;
using eCommerce.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace eCommerce.Core.Mappers
{
    public class ApplicationUserMappingProfile : Profile
    {
        public ApplicationUserMappingProfile()
        {
            CreateMap<ApplicationUser, AuthenticationResponse>()
                .ForMember(x => x.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(x => x.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(x => x.PersonName, opt => opt.MapFrom(src => src.PersonName))
                .ForMember(x => x.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(x => x.Success, opt => opt.Ignore())
                .ForMember(x => x.Token, opt => opt.Ignore());
        }
    }
}
