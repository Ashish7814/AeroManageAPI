using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Tickets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{bookingId}/pnr")]
        public async Task<IActionResult> GetPNR(int bookingId)
        {
            try
            {
                var data = await _mediator.Send(new GetPNRDetailsQuery(bookingId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{bookingId}/e-ticket")]
        public async Task<IActionResult> GenerateETicket(int bookingId)
        {
            try
            {
                var data = await _mediator.Send(new GenerateETicketPDFQuery(bookingId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{bookingId}/qr-code")]
        public async Task<IActionResult> GenerateQRCode(int bookingId, [FromQuery] int? passengerId = null)
        {
            try
            {
                var data = await _mediator.Send(new GenerateQRCodeQuery(bookingId, passengerId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost("{bookingId}/email")]
        public async Task<IActionResult> EmailTicket(int bookingId, [FromBody] EmailTicketDto dto)
        {
            try
            {
                var data = await _mediator.Send(new EmailTicketCommand(bookingId, dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{bookingId}/print")]
        public async Task<IActionResult> GetPrintableTicket(int bookingid)
        {
            try
            {
                var data = await _mediator.Send(new GetPrintableTicketQuery(bookingid));
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
