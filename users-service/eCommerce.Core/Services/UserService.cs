using AutoMapper;
using eCommerce.API.Controllers;
using eCommerce.Core.DTO;
using eCommerce.Core.Entity;
using eCommerce.Core.RepositoryContracts;
using eCommerce.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.Core.Services
{
    internal class UserService(IUserRepository userRepository, IMapper mapper) : IUserService
    {
        private readonly IUserRepository userRepository = userRepository;
        private readonly IMapper mapper = mapper;

        public async Task<UserDTO> GetUserByUserID(Guid userID)
        {
            ApplicationUser? user = await userRepository.GetUserByUserID(userID);
            return mapper.Map<UserDTO>(user);
        }

        public async Task<AuthenticationResponse?> Login(LoginRequest loginRequest)
        {
            ApplicationUser? user = await userRepository.GetUserByEmailAndPassword(loginRequest.Email, loginRequest.Password);
            if (user == null)
            {
                return null;
            }
            else
            {
                //return new AuthenticationResponse(user.UserId,
                //                                  user.Email,
                //                                  user.PersonName,
                //                                  user.Gender,
                //                                  user.ToString(),
                //                                  true);
                return mapper.Map<AuthenticationResponse>(user) with { Token = "Token", Success = true };
            }
        }

        public async Task<AuthenticationResponse?> Register(RegisterRequest registerRequest)
        {
            ApplicationUser user = new()
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password,
                Gender = registerRequest.Gender.ToString(),
                PersonName = registerRequest.PersonName,
            };
            ApplicationUser? registeredUser = await userRepository.AddUser(user);
            if (registeredUser is null)
            {
                return null;
            }
            else
            {
                //return new AuthenticationResponse(registeredUser.UserId,
                //                                  registeredUser.Email,
                //                                  registeredUser.PersonName,
                //                                  registeredUser.Gender,
                //                                  registeredUser.ToString(),
                //                                  true);
                return mapper.Map<AuthenticationResponse>(registeredUser) with { Token = "Token", Success = true };
            }
        }
    }
}
