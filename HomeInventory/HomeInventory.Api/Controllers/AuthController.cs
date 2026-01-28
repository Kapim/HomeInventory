using HomeInventory.Application.Models;
using HomeInventory.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace HomeInventory.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(ILoginUseCase auth) : ControllerBase
    {
        private readonly ILoginUseCase _auth = auth;

        [HttpPost(Name = "Login")]
        public async Task<ActionResult<LoginResult>> Login(LoginRequest request) 
        {
            try
            {
                var result = await _auth.LoginAsync(request.UserName, request.Password);
                return Ok(new LoginResult(result.Id, result.UserRole));
            } catch (InvalidCredentialException) {
                return Unauthorized();
            }
        }
    }
}