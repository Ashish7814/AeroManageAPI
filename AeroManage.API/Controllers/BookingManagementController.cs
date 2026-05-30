using AeroManage.BookingManagement.Application.Commands.Bookings;
using AeroManage.BookingManagement.Application.Commands.Date;
using AeroManage.BookingManagement.Application.Commands.Passengers;
using AeroManage.BookingManagement.Application.Commands.Seats;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Bookings;
using AeroManage.BookingManagement.Application.Queries.Refund;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/booking-management")]
    [ApiController]
    [Authorize]
    public class BookingManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserBookings(int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var data = await _mediator.Send(new GetUserBookingsQuery(userId, pageNumber, pageSize));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingDetails(int bookingId)
        {
            try
            {
                var data = await _mediator.Send(new GetBookingByIdQuery(bookingId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPut("{bookingId}/change-date")]
        public async Task<IActionResult> ChangeDate(int bookingId, [FromBody] ChangeDateDto dto)
        {
            try
            {
                var data = await _mediator.Send(new ChangeDateCommand(bookingId, dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPut("{bookingId}/update-passenger")]
        public async Task<IActionResult> UpdatePassenger(int bookingId, [FromBody] UpdatePassengerDetailsDto dto)
        {
            try
            {
                var data = await _mediator.Send(new UpdatePassengerDetailsCommand(bookingId, dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPut("{bookingId}/change-seat")]
        public async Task<IActionResult> ChangeSeat(int bookingId, [FromBody] ChangeSeatDto dto)
        {
            try
            {
                var data = await _mediator.Send(new ChangeSeatCommand(bookingId, dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost("{bookingId}/cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId, [FromBody] CancelBookingRequestDto dto)
        {
            try
            {
                var data = await _mediator.Send(new CancelBookingCommand(bookingId, dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);

            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost("{bookingId}/refund")]
        public async Task<IActionResult> RequestRefund(int bookingId, [FromBody] RefundRequestDto dto)
        {
            try
            {
                var data = await _mediator.Send(new RequestRefundCommand(bookingId, dto));
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
