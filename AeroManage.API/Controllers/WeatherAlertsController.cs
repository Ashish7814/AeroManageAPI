using AeroManage.FlightManagement.Application.Commands.Weather;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Weather;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/weather-alerts")]
    [ApiController]
    [Authorize]
    public class WeatherAlertsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WeatherAlertsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,AirlineStaff,Weather")]
        public async Task<IActionResult> CreateAlert([FromBody] CreateWeatherAlertDto dto)
        {
            try
            {
                var data = await _mediator.Send(new CreateWeatherAlertCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("active")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveAlerts([FromQuery] int? airportId = null)
        {
            try
            {
                var data = await _mediator.Send(new GetActiveWeatherAlertsQuery(airportId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{alertId}/impacted-flights")]
        public async Task<IActionResult> GetImpactedFlights(int alertId)
        {
            try
            {
                var data = await _mediator.Send(new GetWeatherImpactedFlightsQuery(alertId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpDelete("{alertId}")]
        public async Task<IActionResult> DeactivateAlert(int alertId)
        {
            try
            {
                var data = await _mediator.Send(new DeactivateWeatherAlertCommand(alertId));
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
