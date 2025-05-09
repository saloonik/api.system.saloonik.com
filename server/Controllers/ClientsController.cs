using beautysalon.Database.Models;
using beautysalon.Logic.DTOs;
using beautysalon.Logic.DTOs.ServerResponse;
using beautysalon.Logic.Services.ClientService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace beautysalon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpPost("Add")]
        [Authorize(Roles = "Owner,Staff")]
        public async Task<IActionResult> AddClient([FromBody] ClientDTO client)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var accessToken = HttpContext.Request.Headers["Authorization"]
                    .ToString()
                    .Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

                if (string.IsNullOrWhiteSpace(accessToken))
                    return Unauthorized(new ServerResponse
                    {
                        IsSuccess = false,
                        ResultTitle = "Unauthorized",
                        ResultDescription = "Missing or invalid token.",
                        StatusCode = 401,
                        StatusMessage = "Unauthorized"
                    });

                var result = await _clientService.AddClientAsync(client, accessToken);

                return StatusCode(result.StatusCode, new ServerResponse
                {
                    IsSuccess = result.IsSuccess,
                    ResultTitle = result.ResultTitle,
                    ResultDescription = result.ResultDescription,
                    StatusCode = result.StatusCode,
                    StatusMessage = result.StatusMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ServerResponse
                {
                    IsSuccess = false,
                    ResultTitle = "Internal Server Error",
                    ResultDescription = ex.Message,
                    StatusCode = 500,
                    StatusMessage = "An unexpected error occurred."
                });
            }
        }
    }
}
