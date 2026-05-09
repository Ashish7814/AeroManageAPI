using AeroManage.BookingManagement.Application.Commands.Assistance;
using AeroManage.BookingManagement.Application.Commands.Bookings;
using AeroManage.BookingManagement.Application.Commands.Meals;
using AeroManage.BookingManagement.Application.Commands.Passengers;
using AeroManage.BookingManagement.Application.Commands.Seats;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Bookings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequestDto dto)
        {
            var data = await _mediator.Send(new CreateBookingCommand(dto));
            if (data == null)
                return BadRequest();
            return Ok(data);
            
        }

        [HttpPost("{bookingId}/passengers")]
        public async Task<IActionResult> AddPassenger(int bookingId, [FromBody] AddPassengerToBookingDto dto)
        {
            var data = await _mediator.Send(new AddPassengerCommand(bookingId, dto));
            if (data == null)
                return BadRequest();
            return Ok(data);

        }

        [HttpPost("{bookingId}/meal-preference")]
        public async Task<IActionResult> AddMealPreference(int bookingId, [FromBody] MealPreferenceDto dto)
        {
            try
            {
                var data = await _mediator.Send(new AddMealPreferenceCommand(bookingId, dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost("{bookingId}/special-assistance")]
        public async Task<IActionResult> AddSpecialAssistance(int bookingId, [FromBody] SpecialAssistanceDto dto)
        {
            try
            {
                var data = await _mediator.Send(new AddSpecialAssistanceCommand(bookingId, dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost("{bookingId}/addons")]
        public async Task<IActionResult> AddBookingAddons(int bookingId, [FromBody] BookingAddonsDto dto)
        {
            try
            {
                var data = await _mediator.Send(new AddBookingAddonsCommand(bookingId, dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        [AllowAnonymous]
        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingSummary(int bookingId)
        {
            try
            {
                var data = await _mediator.Send(new GetBookingSummaryQuery(bookingId));
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
