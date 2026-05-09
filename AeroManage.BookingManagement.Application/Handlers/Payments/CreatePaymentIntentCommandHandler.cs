using AeroManage.BookingManagement.Application.Commands.Payment;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Services.Interfaces;
using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Payments
{
    public class CreatePaymentIntentCommandHandler : IRequestHandler<CreatePaymentIntentCommand, ApiResponse<PaymentResultDto>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IStripePaymentService _stripeService;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ILogger<CreatePaymentIntentCommandHandler> _logger;

        public CreatePaymentIntentCommandHandler(
            IBookingRepository bookingRepo,
            IStripePaymentService stripeService,
            IPaymentRepository paymentRepo,
            ILogger<CreatePaymentIntentCommandHandler> logger)
        {
            _bookingRepo = bookingRepo;
            _stripeService = stripeService;
            _paymentRepo = paymentRepo;
            _logger = logger;
        }
        public async Task<ApiResponse<PaymentResultDto>> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var booking = await _bookingRepo.GetBookingByIdAsync(request.dto.BookingId);

                if (booking == null)
                {
                    return ApiResponse<PaymentResultDto>.ErrorResponse("Booking not found");
                }

                // Create payment intent
                var metadata = new Dictionary<string, string>
                {
                    { "booking_id", booking.BookingId.ToString() },
                    { "booking_reference", booking.BookingReference },
                    { "pnr", booking.PNR },
                    { "email", booking.BookingEmail }
                };

                var intent = await _stripeService.CreatePaymentIntentAsync(
                    booking.BookingId,
                    request.dto.Amount,
                    request.dto.Currency,
                    metadata
                );

                // Save payment record
                var payment = new Payment
                {
                    BookingId = booking.BookingId,
                    PaymentReference = Guid.NewGuid().ToString(),
                    PaymentIntentId = intent.Id,
                    Amount = request.dto.Amount,
                    Currency = request.dto.Currency,
                    PaymentStatus = "Pending",
                    PaymentMethod = "Stripe",
                    Metadata = System.Text.Json.JsonSerializer.Serialize(metadata)
                };

                await _paymentRepo.CreatePaymentAsync(payment);

                var result = new PaymentResultDto
                {
                    Success = true,
                    PaymentIntentId = intent.Id,
                    ClientSecret = intent.ClientSecret,
                    PaymentStatus = "Pending",
                    Message = "Payment intent created successfully"
                };

                return ApiResponse<PaymentResultDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating payment intent for booking {request.dto.BookingId}");
                return ApiResponse<PaymentResultDto>.ErrorResponse("Failed to create payment", new[] { ex.Message });
            }
        }
    }
}
