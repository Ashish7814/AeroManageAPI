using AeroManage.FlightManagement.Application.Commands.Routes;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Routes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RoutesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("addFlightRoute")]
        [Authorize(Roles = "Admin,AirlineStaff")]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDto dto)
        {
            try
            {
                var data = await _mediator.Send(new CreateRouteCommand(dto));
                if (data == null)
                    return BadRequest("Unable to create Route for Aircraft");
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getAllFilghtRoute")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRoutes(RouteSearchDto dto)
        {
            try
            {
                var data = await _mediator.Send(new GetAllRoutesQuery(dto));
                if (data == null)
                    return BadRequest("Unable to find Aircraft Route");
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getFlightRoute/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRouteById(int id)
        {
            var result = await _mediator.Send(new GetRouteByIdQuery(id));
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("{routeId}/layovers")]
        [Authorize(Roles = "Admin,AirlineStaff")]
        [ProducesResponseType(typeof(ApiResponse<RouteLayoverDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddLayover(int routedId, [FromBody] AddRouteLayoverDto dto)
        {
            try
            {

                var data = await _mediator.Send(new AddRouteLayoverCommand(dto));
                if (data == null)
                    return BadRequest("Unable to create Route layovers");
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{routeId}/layovers")]
        public async Task<IActionResult> GetLayovers(int routeId)
        {
            try
            {
                var data = await _mediator.Send(new GetRouteLayoversQuery(routeId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("{routeId}/with-layovers")]
        public async Task<IActionResult> GetRouteWithLayovers(int routeId)
        {
            try
            {
                var data = await _mediator.Send(new GetRouteWithLayoversQuery(routeId));
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
