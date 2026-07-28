using eCommerce.Core.DTO;
using eCommerce.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserService userService) : ControllerBase
    {
        private readonly IUserService userService = userService;
        [HttpPost("register")]
        public async Task<IActionResult> Register(Core.DTO.RegisterRequest registerRequest)
        {
            if (registerRequest is null)
            {
                return BadRequest("Invalid registration data");
            }
            AuthenticationResponse? authenticationResponse = await userService.Register(registerRequest);
            if (authenticationResponse is null || !authenticationResponse.Success)
            {
                return BadRequest(authenticationResponse);
            }
            return Ok(authenticationResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(Core.DTO.LoginRequest loginRequest)
        {
            if (loginRequest is null)
            {
                return BadRequest("Invalid registration data");
            }
            AuthenticationResponse? authenticationResponse = await userService.Login(loginRequest);
            if (authenticationResponse is null || !authenticationResponse.Success)
            {
                return Unauthorized(authenticationResponse);
            }
            return Ok(authenticationResponse);
        }
    }
}
