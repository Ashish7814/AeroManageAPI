using AeroManage.BookingManagement.Application.Commands.BoardingPass;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.BoardingPass;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/boarding-passes")]
    [ApiController]
    public class BoardingPassController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BoardingPassController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("generate")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GenerateBoardingPass([FromQuery] BoardingPassRequestDto dto)
        {
            try
            {
                var data = await _mediator.Send(new GenerateBoardingPassCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getBoardingPass")]
        [Authorize]
        public async Task<IActionResult> GetBoardingPass(int? BoardingPassId, string BoardingPassNumber, int? BookingPassengerId)
        {
            try
            {
                var data = await _mediator.Send(new GetBoardingPassQuery(BoardingPassId, BoardingPassNumber, BookingPassengerId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost("scan")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> ScanBoardingPass(string BoardingPassNumber, string ScannedBy)
        {
            try
            {
                var data = await _mediator.Send(new ScanBoardingPassCommand(BoardingPassNumber, ScannedBy));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("flight/{flightId}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetFlightBoardingPasses(int flightId)
        {
            try
            {
                var data = await _mediator.Send(new GetFlightBoardingPassesQuery(flightId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPut("flight/{flightId}/gate")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateBoardingPassGate(BoardingPassGateRequestDto dto)
        {
            try
            {
                var data = await _mediator.Send(new UpdateBoardingPassGateCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

      /*  [HttpPost("booking/{bookingId}/generate")]
        [Authorize(Roles = "Admin,Staff")]
        public Task<IActionResult> GenerateBoardingPassesForBooking()*/
    }
}
