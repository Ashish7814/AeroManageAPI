using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Flights;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/flights/dashboard")]
    [ApiController]
    [Authorize]
    public class FlightDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FlightDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get real-time flight dashboard (cached - 1 min)
        /// </summary>
        /// <param name="airportId">Filter by airport</param>
        /// <param name="date">Filter by date (default: today)</param>
        /// <param name="status">Filter by status</param>
        /// <returns>Flight dashboard data</returns>
        [HttpGet]
        public async Task<IActionResult> GetDashboard(FlightDashboardQueryDto dto)
        {
            try
            {
                var data = await _mediator.Send(new GetFlightDashboardQuery(dto));
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
