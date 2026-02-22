using HomeInventory.Application.Models;
using HomeInventory.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using HomeInventory.Contracts;
using System.Security.Authentication;
using HomeInventory.Api.Mapping;
using HomeInventory.Application.Exceptions;

namespace HomeInventory.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(ILoginUseCase auth, IRegisterUseCase registration) : ControllerBase
    {
        private readonly ILoginUseCase _auth = auth;
        private readonly IRegisterUseCase _registration = registration;

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request) 
        {
            try
            {
                var result = await _auth.LoginAsync(request.UserName, request.Password);
                return Ok(new LoginResponseDto(result.Token, Mapping.UserRoleMapping.Map(result.UserRole)));
            } catch (InvalidCredentialException) {
                return Unauthorized();
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<bool>> Register(RegisterRequestDto registerRequest)
        {
            try
            {
                await _registration.RegisterAsync(registerRequest.UserName, registerRequest.Password, UserRoleMapping.Map(registerRequest.UserRole));
                return true;
            } catch (DuplicateUserNameException)
            {
                return false;
            }
        }
    }
}