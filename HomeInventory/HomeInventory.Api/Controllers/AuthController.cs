using HomeInventory.Application.Models;
using HomeInventory.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using HomeInventory.Contracts;
using System.Security.Authentication;

namespace HomeInventory.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(ILoginUseCase auth) : ControllerBase
    {
        private readonly ILoginUseCase _auth = auth;

        [HttpPost(Name = "Login")]
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
    }
}