using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Airlines;
using AeroManage.BookingManagement.Application.Queries.Fares;
using AeroManage.BookingManagement.Application.Queries.Flights;
using AeroManage.BookingManagement.Application.Queries.Seats;
using AeroManage.FlightManagement.Application.Queries.Flights;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/flights")]
    [ApiController]
    public class FlightSearchController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FlightSearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("searchFlight")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchFlights([FromQuery] AeroManage.FlightManagement.Application.DTOs.FlightSearchDto dto)
        {
            var data = await _mediator.Send(new SearchFlightsQuery(dto));
            if (data == null)
                return BadRequest("Unable to find Filght");
            return Ok(data);
        }

        [HttpGet("calendar-fares")]
        public async Task<IActionResult> GetCalendarFares([FromQuery] CalendarFaresDto dto)
        {
            try
            {
                var data = await _mediator.Send(new GetCalendarFaresQuery(dto));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{flightId}")]
        public async Task<IActionResult> GetFlightDetails(int flightId)
        {
            try
            {
                var data = await _mediator.Send(new GetFlightDetailsQuery(flightId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{flightId}/seat-availability")]
        public async Task<IActionResult> GetSeatAvailability(int flightId, [FromQuery] string seatClass = null)
        {
            try
            {
                var data = await _mediator.Send(new GetSeatAvailabilityQuery(flightId, seatClass));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("airlines")]
        public async Task<IActionResult> GetAirlines()
        {
            try
            {
                var data = await _mediator.Send(new GetAirlinesQuery());
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
