using AeroManage.BookingManagement.Application.Commands.Promocode;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Promocode;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/promo-codes")]
    [ApiController]
    public class PromoCodeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PromoCodeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("validate")]
        public async Task<IActionResult> ValidatePromoCode([FromQuery] ValidatePromoCodeDto dto)
        {
            try
            {
                var data = await _mediator.Send(new ValidatePromoCodeQuery(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActivePromoCodes()
        {
            try
            {
                var data = await _mediator.Send(new GetActivePromoCodesQuery());
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        [HttpGet("{code}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPromoCodeByCode(string code)
        {
            try
            {
                var data = await _mediator.Send(new GetPromoCodeByCodeQuery(code));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePromoCode([FromBody] PromoCodeDto dto)
        {
            try
            {
                var data = await _mediator.Send(new CreatePromoCodeCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePromoCode([FromBody] PromoCodeDto dto)
        {
            try
            {
                var data = await _mediator.Send(new UpdatePromoCodeCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivatePromoCode(int promoCodeId)
        {
            try
            {
                var data = await _mediator.Send(new DeactivatePromoCodeCommand(promoCodeId));
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
