using AeroManage.FlightManagement.Application.Commands.Flights;
using AeroManage.FlightManagement.Application.Commands.Flights.FlightSchedule;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.FlightSchedule;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AeroManage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FlightScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FlightScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("templates")]
        [Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateFlightScheduleTemplateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var data = await _mediator.Send(new CreateFlightScheduleTemplateCommand(dto));
                if (data == null)
                    return BadRequest("Unable to create Template for flight schedule");
                return Ok(data);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getTemplates")]
        [Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> GetAllTemplates([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool? isActive = null)
        {
            try
            {
                var data = await _mediator.Send(new GetAllScheduleTemplatesQuery(pageNumber, pageSize, isActive));
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
