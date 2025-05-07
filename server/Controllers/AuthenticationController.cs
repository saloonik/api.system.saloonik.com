using beautysalon.AuthContracts;
using beautysalon.Contracts;
using beautysalon.Database.Models;
using beautysalon.Logic.DTOs.ServerResponse;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace beautysalon.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthenticationController (IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("/register")]
        public async Task<ActionResult> RegisterAsync (RegisterRequest authRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.RegisterAsync(authRequest);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServerResponse
                {
                    IsSuccess = false,
                    ResultTitle = "Error",
                    ResultDescription = ex.Message,
                    StatusCode = 500,
                    StatusMessage = "An unexpected error occurred."
                });
            }
        }

        [HttpPost("/login")]
        public async Task<ActionResult> LoginAsync (LoginRequest authRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _authService.LoginAsync(authRequest);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServerResponse
                {
                    IsSuccess = false,
                    ResultTitle = "Error",
                    ResultDescription = ex.Message,
                    StatusCode = 500,
                    StatusMessage = "An unexpected error occurred."
                });
            }
        }

        [HttpPost("/refresh")]
        public async Task<ActionResult> GetAccessTokenAsync([FromHeader] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Refresh token is required.");

            try
            {
                var result = await _authService.GetAccessTokenFromRefreshTokenAsync(refreshToken);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServerResponse
                {
                    IsSuccess = false,
                    ResultTitle = "Error",
                    ResultDescription = ex.Message,
                    StatusCode = 500,
                    StatusMessage = "An unexpected error occurred."
                });
            }
        }

    }
}
