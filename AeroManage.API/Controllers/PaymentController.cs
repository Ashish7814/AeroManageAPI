using AeroManage.BookingManagement.Application.Commands.Bookings;
using AeroManage.BookingManagement.Application.Commands.Payment;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Invoices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace AeroManage.API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("paymentIntent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentDto dto)
        {
            try
            {
                var data = await _mediator.Send(new CreatePaymentIntentCommand(dto));
                if (data == null)
                    return BadRequest("Unable to create payment Intent");

                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /*[HttpPost("create-setup-intent")]
        public Task<IActionResult> CreateSetupIntent()*/

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET")
                );

                if (stripeEvent.Type == "Succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    var bookingId = int.Parse(paymentIntent.Metadata["booking_id"]);

                    // Confirm booking
                    var command = new ConfirmBookingCommand(
                        new ConfirmPaymentDto
                        {
                            BookingId = bookingId,
                            PaymentIntentId = paymentIntent.Id
                        });

                    await _mediator.Send(command);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

       /* [HttpPost("initiate")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResultDto>), 200)]
        public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new InitiatePaymentCommand
            {
                BookingId = dto.BookingId,
                PaymentMethod = dto.PaymentMethod,
                CardToken = dto.CardToken,
                SaveCard = dto.SaveCard
            };

            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }*/

        [HttpPost("confirm")]
        [ProducesResponseType(typeof(ApiResponse<PaymentResultDto>), 200)]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentDto dto)
        {
            try
            {
                var data = await _mediator.Send(new ConfirmBookingCommand(dto));
                if (data == null)
                    return BadRequest();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

       /* [HttpGet("{bookingId}/status")]
        public async Task<IActionResult> GetPaymentStatus(int bookingId)
        {
            try
            {
                var data = await _mediator.Send(new GetPaymentStatusQuery(bookingId));
                if (data == null)
                    return NotFound();
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }*/

       
        [HttpGet("{bookingId}/invoice")]
        public async Task<IActionResult> DownloadInvoice(int bookingId)
        {
            try
            {
                var data = await _mediator.Send(new DownloadInvoiceQuery(bookingId));
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
