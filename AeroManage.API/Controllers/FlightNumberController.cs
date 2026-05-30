using AeroManage.FlightManagement.Application.Commands.Flights.FlightNumber;
using AeroManage.FlightManagement.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/flight-numbers")]
    [ApiController]
    [Authorize(Roles = "Admin,AirlineStaff")]
    public class FlightNumberController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FlightNumberController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateFlightNumber([FromBody] GenerateFlightNumberDto dto)
        {
            try
            {
                var data = await _mediator.Send(new GenerateFlightNumberCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
