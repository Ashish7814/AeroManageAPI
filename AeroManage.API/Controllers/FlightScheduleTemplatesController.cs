using AeroManage.FlightManagement.Application.Commands.Flights.FlightSchedule;
using AeroManage.FlightManagement.Application.Commands.Template;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.FlightSchedule;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AeroManage.API.Controllers
{
    [Route("api/")]
    [ApiController]
    //[Authorize]
    public class FlightScheduleTemplatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FlightScheduleTemplatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("createTemplates")]
        [Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateFlightScheduleTemplateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var data = await _mediator.Send(new CreateFlightScheduleTemplateCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("schedule-templates")]
        //[Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> GetAllTemplates(
            [FromQuery] int pageNumber = 1,
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

        [HttpGet("templates/{id}")]
        [Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> GetTemplateById(int id)
        {
            try
            {
                var data = await _mediator.Send(new GetScheduleTemplateByIdQuery(id));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost("schedule-templates/{id}/generate")]
        [Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> GenerateFlights(int id, [FromBody] GenerateFlightsDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var data = await _mediator.Send(new GenerateFlightsFromTemplateCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpDelete("templates/{id}")]
        [Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> DeactivateTemplate(int id)
        {
            try
            {
                var data = await _mediator.Send(new DeactivateScheduleTemplateCommand(id));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
