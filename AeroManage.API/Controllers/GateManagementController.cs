using AeroManage.FlightManagement.Application.Commands.Flights.FlightGate;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Flights.FlightGate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/flights/{flightId}/gates")]
    [ApiController]
    [Authorize(Roles = "Admin,AirlineStaff")]
    public class GateManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GateManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("assignGate")]
        public async Task<IActionResult> AssignGate([FromBody] AssignGateDto dto)
        {
            try
            {
                var data = await _mediator.Send(new AssignFlightGateCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPut("changeGate")]
        public async Task<IActionResult> ChangeGate(ChangeGateDto dto)
        {
            try
            {
                var data = await _mediator.Send(new ChangeFlightGateCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getAssignGate")]
        public async Task<IActionResult> GetGateAssignments(int flightId)
        {
            try
            {
                var data = await _mediator.Send(new GetFlightGateAssignmentsQuery(flightId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
