using AeroManage.FlightManagement.Application.Commands.Flights.Notification;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Flights.Notification;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AeroManage.API.Controllers
{
    [Route("api/flights")]
    [ApiController]
    //[Authorize]
    public class FlightNotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FlightNotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{flightId}/notifications")]
        [Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> CreateNotification(int flightId, [FromBody] CreateFlightNotificationDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var data = await _mediator.Send(new CreateFlightNotificationCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{flightId}/notifications")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNotifications(int flightId)
        {
            try
            {
                var data = await _mediator.Send(new GetFlightNotificationsQuery(flightId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPatch("notifications/{notificationId}/resolve")]
        //[Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> ResolveNotification(int notificationId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var data = await _mediator.Send(new ResolveFlightNotificationCommand(notificationId, userId));
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
