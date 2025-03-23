using beautysalon.Contracts;
using beautysalon.Logic.DTOs.ServerResponse;
using Microsoft.AspNetCore.Mvc;

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

                var serverResponse = new ServerResponse
                {
                    IsSuccess = result.IsSuccess,
                    ResultTitle = result.ResultTitle,
                    ResultDescription = result.ResultDescription,
                    StatusCode = result.StatusCode,
                    StatusMessage = result.StatusMessage
                };
                    return StatusCode (serverResponse.StatusCode, serverResponse);
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
