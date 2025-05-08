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
        public async Task<ActionResult> AddClient([FromBody] ClientDTO client)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader))
                return Unauthorized("Missing token");

            var token = authHeader.Replace("Bearer ", "");

            try
            {
                var result = await _clientService.AddClientAsync(client, token);

                var response = new ServerResponse
                {
                    IsSuccess = result.IsSuccess,
                    ResultTitle = result.ResultTitle,
                    ResultDescription = result.ResultDescription,
                    StatusCode = result.StatusCode,
                    StatusMessage = result.StatusMessage
                };

                return StatusCode(result.StatusCode, response);
            }
            catch (Exception Ex)
            {
                return StatusCode(500, new ServerResponse
                {
                    IsSuccess = false,
                    ResultTitle = "Error",
                    ResultDescription = Ex.Message,
                    StatusCode = 500,
                    StatusMessage = "An unexpected error occurred."
                });
            }
        
        }

    }
}
