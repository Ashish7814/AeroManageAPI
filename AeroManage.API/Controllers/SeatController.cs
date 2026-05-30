using AeroManage.BookingManagement.Application.Commands.Seats;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Seats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/seats")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SeatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{seatId:int}")]
        public async Task<IActionResult> GetSeatById(int seatId)
        {
            try
            {
                var data = await _mediator.Send(new GetSeatByIdQuery(seatId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("flight/{flightId:int}/seat-number/{seatNumber}")]
        public async Task<IActionResult> GetSeatByNumber(int flightId, string seatNumber)
        {
            try
            {
                var data = await _mediator.Send(new GetSeatByNumberQuery(flightId, seatNumber));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("flight/{flightId:int}/available")]
        public async Task<IActionResult> GetAvailableSeatsByClass(int flightId, [FromQuery] string seatClass = "Economy")
        {
            try
            {
                var data = await _mediator.Send(new GetAvailableSeatsByClassQuery(flightId, seatClass));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("flight/{flightId:int}/map")]
        public async Task<IActionResult> GetSeatMap(int flightId)
        {
            try
            {
                var data = await _mediator.Send(new GetSeatMapQuery(flightId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        [HttpPost("{bookingId}/seat-selection")]
        public async Task<IActionResult> SelectSeat(int bookingId, [FromBody] SeatSelectionDto dto)
        {
            try
            {
                var data = await _mediator.Send(new SelectSeatCommand(bookingId, dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpDelete("reservations/{reservationId:int}")]
        public async Task<IActionResult> ReleaseReservation(int reservationId,[FromQuery] int flightId)
        {
            try
            {
                var data = await _mediator.Send(new ReleaseSeatReservationCommand(reservationId, flightId));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost("reservations/release-expired")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReleaseExpiredReservations()
        {
            try
            {
                var data = await _mediator.Send(new ReleaseExpiredReservationsCommand());
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
