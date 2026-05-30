using AeroManage.FlightManagement.Application.Commands.Aircraft;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Aircraft;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AircraftController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AircraftController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("")]
        [Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> CreateAircraft([FromBody] CreateAircraftDto dto)
        {
            try
            {
                var data = await _mediator.Send(new CreateAircraftCommand(dto));
                if (data == null)
                    return BadRequest("Unable to create aircraft");
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getAllAircraft")]
        //[Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> GetAllAircraft([FromQuery] AircraftSearchDto dto)
        {
            try
            {
                var data = await _mediator.Send(new GetAllAircraftQuery(dto));
                if (data == null)
                    return NotFound("Unalbe to find Aircraft");
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getAircraftById/{id}")]
        public async Task<IActionResult> GetAircraftById(int id)
        {
            try
            {
                var data = await _mediator.Send(new GetAircraftByIdQuery(id));
                if (data == null)
                    return NotFound("Unalbe to find Aircraft");
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPatch("aircraftStatus")]
        [Authorize(Roles = "Admin,AirlineStaff,MaintenanceCrew")]
        public async Task<IActionResult> UpdateAircraftStatus([FromBody] UpdateAircraftStatusDto dto)
        {
            try
            {
                var data = await _mediator.Send(new UpdateAircraftStatusCommand(dto));
                if (data == null)
                    return BadRequest("Unalbe to updata Aircraft Status");
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
