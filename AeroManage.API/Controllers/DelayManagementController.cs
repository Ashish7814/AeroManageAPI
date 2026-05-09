using AeroManage.FlightManagement.Application.Commands.Flights.FlightDelay;
using AeroManage.FlightManagement.Application.Commands.Flights.FlightNumber;
using AeroManage.FlightManagement.Application.Commands.Flights.FlightStatus;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Flights;
using AeroManage.FlightManagement.Application.Queries.Flights.FlightDelay;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AeroManage.API.Controllers
{
    [Route("api/delay")]
    [ApiController]
    //[Authorize(Roles = "Admin,AirlineStaff")]
    public class DelayManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DelayManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("reportDelay")]
        public async Task<IActionResult> ReportDelay([FromBody] ReportFlightDelayDto dto)
        {
            try
            {
                var data = await _mediator.Send(new ReportFlightDelayCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{id}/delay-reasons")]
        public async Task<IActionResult> GetDelayHistory(int id)
        {
            try
            {
                var data = await _mediator.Send(new GetFlightDelayHistoryQuery(id));
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
