using AeroManage.FlightManagement.Application.Commands.Flights.FlightStatus;
using AeroManage.FlightManagement.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AeroManage.API.Controllers
{
    [Route("api/flights/{flightId}/boarding")]
    [ApiController]
    [Authorize(Roles = "Admin,AirlineStaff")]
    public class BoardingStatusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BoardingStatusController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPatch]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateBoardingStatus([FromBody] UpdateBoardingStatusDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var data = await _mediator.Send(new UpdateBoardingStatusCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

    }
}
