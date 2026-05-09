using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Pricing;
using AeroManage.BookingManagement.Application.Queries.Promocode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/pricing")]
    [ApiController]
    public class PricingController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PricingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculatePrice([FromBody] PricingCalculationRequestDto dto)
        {
            try
            {
                var data = await _mediator.Send(new CalculatePricingQuery(dto));
                if (data == null)
                    return BadRequest("Unable to calculate Price");
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("calculate-quick")]
        public async Task<IActionResult> CalculateQuickPricing([FromBody] PricingCalculationRequestDto dto)
        {
            try
            {
                var data = await _mediator.Send(new CalculatePricingQuery(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw new Exception("Error calculating quick pricing" + ex.Message);
            }
        }

        [HttpPost("promo-code/validate")]
        public async Task<IActionResult> ValidatePromoCode([FromBody] ValidatePromoCodeDto dto)
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
    }
}
