using AeroManage.FlightManagement.Application.Commands.Airport;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Airports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AirportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("createAirport")]
        /*[Authorize(Roles = "Admin,AirlineStaff")]*/
        public async Task<IActionResult> CreateAirport([FromBody] CreateAirportDto dto)
        {
            try
            {
                var data = await _mediator.Send(new CreateAirportCommand(dto));
                if (data == null)
                    return BadRequest("Unable to create airport");

                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getAllAirports")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAirports([FromQuery] AirportSearchDto dto)
        {
            try
            {
                var data = await _mediator.Send(new GetAllAirportsQuery(dto));
                if (data == null)
                    return NotFound("Unable to get Airports");

                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getAirportbyId/{id}")]
        public async Task<IActionResult> GetAirportById(int id)
        {
            try
            {
                var data = await _mediator.Send(new GetAirportByIdQuery(id));
                if (data == null)
                    return NotFound("Unable to find AirportbyId");
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

    }
}
