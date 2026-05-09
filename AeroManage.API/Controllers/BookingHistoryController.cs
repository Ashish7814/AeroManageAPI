using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Histories;
using AeroManage.BookingManagement.Application.Queries.Statistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/")]
    [ApiController]
    public class BookingHistoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BookingHistoryController> _logger;

        public BookingHistoryController(IMediator mediator, ILogger<BookingHistoryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("booking-history")]
        //[Authorize(Roles = "Admin,Staff,HR")]
        public async Task<IActionResult> GetBookingHistory([FromQuery] BookingHistoryFilterDto dto)
       {
            try
            {
                var data = await _mediator.Send(new GetBookingHistoryQuery(dto));
                if(data.Bookings == null || !data.Bookings.Any())
                    return NotFound();
                return Ok(new { bookings = data.Bookings, totalCount = data.TotalCount });
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieving booking history" + ex.Message);
            }
        }

        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings(BookingHistoryFilterDto dto)
        {
            try
            {
                var data = await _mediator.Send(new GetBookingHistoryByIdQuery(dto));
                if (data.Bookings == null || !data.Bookings.Any())
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieving user bookings" + ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("statistics")]
        //[Authorize(Roles = "Admin,Staff,HR")]
        public async Task<IActionResult> GetStatistics([FromQuery] int? userId = null, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var data = await _mediator.Send(new GetBookingStatisticsQuery(userId, fromDate, toDate));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieving booking statistics" + ex.Message);
            }
        }

        [HttpGet("my-statistics")]
        public async Task<IActionResult> GetMyStatistics([FromQuery] int? userId = null, [FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var data = await _mediator.Send(new GetBookingStatisticsQuery(userId, fromDate, toDate));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieving user statistics" + ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchBooking(BookingHistoryFilterDto dto)
        {
            try
            {
                var data = await _mediator.Send(new GetBookingHistoryQuery(dto));
                if (data.Bookings == null || !data.Bookings.Any())
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw new Exception("Error searching booking");
            }
        }
    }
}
