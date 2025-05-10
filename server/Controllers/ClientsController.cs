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
        public async Task<ActionResult> AddClient([FromBody] ClientDTO client)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var accessToken = HttpContext.Request.Headers["Authorization"]
                    .ToString()
                    .Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

                var result = await _clientService.AddClientAsync(client, accessToken);

                return StatusCode(result.StatusCode, result);
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

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult> DeleteClient([FromRoute] string id)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {

                var token = HttpContext.Request.Headers["Authorization"]
                    .ToString()
                    .Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

                var result = await _clientService.DeleteClientByIdAsync(id, token);

                return StatusCode(result.StatusCode, result);
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
