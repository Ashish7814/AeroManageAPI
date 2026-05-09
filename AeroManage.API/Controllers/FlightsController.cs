using AeroManage.FlightManagement.Application.Commands.Flights;
using AeroManage.FlightManagement.Application.Commands.Flights.FlightCrew;
using AeroManage.FlightManagement.Application.Commands.Flights.FlightStatus;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Flights;
using AeroManage.FlightManagement.Application.Queries.Flights.FlightCrew;
using AeroManage.FlightManagement.Application.Queries.Flights.FlightStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AeroManage.API.Controllers
{
    [Route("api/flights")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FlightsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a new flight
        /// </summary>
        [HttpPost("addFlight")]
        //[Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> CreateFlight([FromBody] CreateFlightDto dto)
        {
            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var result = await _mediator.Send(new CreateFlightCommand(dto));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        ///<summary>
        ///Get Flight
        /// </summary>
        [HttpGet("getFlights")]
        //[Authorize(Roles = "Admin,ArilineStaff")]
        public async Task<IActionResult> GetFlights()
        {
            try
            {
                var data = await _mediator.Send(new GetFlightsQuery());
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Search flights
        /// </summary>
        [HttpPost("searchFlight")]
        //[AllowAnonymous]
        public async Task<IActionResult> SearchFlights([FromBody] FlightSearchDto dto)
        {
            var data = await _mediator.Send(new SearchFlightsQuery(dto));
            if (data == null)
                return BadRequest("Unable to find Filght");
            return Ok(data);
        }

        /// <summary>
        /// Get flight by ID
        /// </summary>
        [HttpGet("GetFlightBy/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFlightById(int id)
        {
            var data = await _mediator.Send(new GetFlightByIdQuery(id));
            if (data == null)
                return BadRequest("Unalbe to find Flight by Id");
            return Ok(data);
        }

        /// <summary>
        /// Update flight status
        /// </summary>
        [HttpPatch("{id}/status")]
        //[Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> UpdateFlightStatus([FromBody] UpdateFlightStatusDto dto)
        {

            //var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

          /*  var command = new UpdateFlightStatusCommand
            {
                FlightId = id,
                Status = dto.Status,
                Reason = dto.Reason,
                ChangedBy = userId
            };*/

            var data = await _mediator.Send(new UpdateFlightStatusCommand(dto));
            if (data == null)
                return BadRequest("Unable to update flight status");
            return Ok(data);
        }

        /// <summary>
        /// Assign crew to flight
        /// </summary>
        [HttpPost("assignFlightCrew/{id}")]
        [Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> AssignFlightCrew([FromBody] AssignFlightCrewDto dto)
        {
            var data = await _mediator.Send(new AssignFlightCrewCommand(dto));
            if (data == null)
                return BadRequest("Unable to assign Flight crew");
            return Ok(data);
        }

        /// <summary>
        /// Get flight crew
        /// </summary>
        [HttpGet("{id}/crew")]
        //[Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> GetFlightCrew(int id)
        {
            var data = await _mediator.Send(new GetFlightCrewQuery(id));
            if (data == null)
                return BadRequest("Unable to find Flight crew");
            return Ok(data);
        }

        /// <summary>
        /// Get flight status history
        /// </summary>
        [HttpGet("{id}/status-history")]
        //[Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> GetFlightStatusHistory(int id)
        {
            var data = await _mediator.Send(new GetFlightStatusHistoryQuery(id));
            if (data == null)
                return BadRequest("unable to find flight status history");
            return Ok(data);
        }

      /*  [HttpGet("{flightId}/seats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSeats(int flightId)
        {
            try
            {
                var data = await _mediator.Send(new GetFlightSeatsQuery(flightId));
                if (data == null)
                    return BadRequest("Unable to get Seat in flight");
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }*/

    }
}
