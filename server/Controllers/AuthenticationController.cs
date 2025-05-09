using beautysalon.AuthContracts;
using beautysalon.Contracts;
using beautysalon.Logic.DTOs.ServerResponse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace beautysalon.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;
     
        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("/register")]
        public async Task<ActionResult> RegisterAsync([FromBody] RegisterRequest authRequest)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.RegisterAsync(authRequest);

                var response = new ServerResponse
                {
                    IsSuccess = result.IsSuccess,
                    ResultTitle = result.ResultTitle,
                    ResultDescription = result.ResultDescription,
                    StatusCode = result.StatusCode,
                    StatusMessage = result.StatusMessage,
                    RefreshToken = result.RefreshToken,
                    Token = result.Token,
                };

                return StatusCode(result.StatusCode, response);
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
        public async Task<ActionResult> LoginAsync([FromBody] LoginRequest authRequest)
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
        [Authorize(Roles = "Owner,Staff")]
        public async Task<ActionResult> GetAccessTokenAsync([FromHeader] string refreshToken)
        {
            try
            {
                var result = await _authService.RefreshAccessTokenAsync(refreshToken);

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
