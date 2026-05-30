using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Hubs;
using AeroManage.BookingManagement.Application.Queries.Refund;
using AeroManage.BookingManagement.Application.Services.Interfaces;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Refunds
{
    public class RequestRefundCommandHandler : IRequestHandler<RequestRefundCommand, ApiResponse<RefundResultDto>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IBookingRepository _extendedRepo;
        private readonly IStripePaymentService _stripeService;
        private readonly ICacheService _cache;
        private readonly IHubContext<BookingHub> _hubContext;
        /*private readonly IMessageQueueService _messageQueue;
        private readonly IBackgroundJobClient _backgroundJobs;*/
        private readonly ILogger<RequestRefundCommandHandler> _logger;

        public RequestRefundCommandHandler(
            IBookingRepository bookingRepo,
            IPaymentRepository paymentRepo,
            IBookingRepository extendedRepo,
            IStripePaymentService stripeService,
            ICacheService cache,
            IHubContext<BookingHub> hubContext,
        /*    IMessageQueueService messageQueue,
            IBackgroundJobClient backgroundJobs,*/
            ILogger<RequestRefundCommandHandler> logger)
        {
            _bookingRepo = bookingRepo;
            _paymentRepo = paymentRepo;
            _extendedRepo = extendedRepo;
            _stripeService = stripeService;
            _cache = cache;
            _hubContext = hubContext;
       /*     _messageQueue = messageQueue;
            _backgroundJobs = backgroundJobs;*/
            _logger = logger;
        }

        public async Task<ApiResponse<RefundResultDto>> Handle(RequestRefundCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get booking
                var booking = await _bookingRepo.GetBookingByIdAsync(request.bookingId);
                if (booking == null)
                {
                    return ApiResponse<RefundResultDto>.ErrorResponse("Booking not found");
                }

                if (booking.BookingStatus == "Cancelled")
                {
                    return ApiResponse<RefundResultDto>.ErrorResponse("Booking already cancelled");
                }

                // Get payment
                var payment = await _paymentRepo.GetPaymentByBookingIdAsync(request.bookingId);
                if (payment == null || payment.PaymentStatus != "Paid")
                {
                    return ApiResponse<RefundResultDto>.ErrorResponse("No valid payment found for refund");
                }

                // Calculate cancellation fee
                var cancellationFee = await _extendedRepo.CalculateCancellationFeeAsync(request.bookingId);
                var refundAmount = request.dto.PartialRefundAmount ?? (booking.TotalAmount - cancellationFee);

                if (refundAmount <= 0)
                {
                    return ApiResponse<RefundResultDto>.ErrorResponse("No refund available - cancellation fee exceeds booking amount");
                }

                // Create refund request in database
                var (refundId, refundReference) = await _extendedRepo.CreateRefundRequestAsync(
                    request.bookingId,
                    payment.PaymentId,
                    refundAmount,
                    cancellationFee,
                    request.dto.RefundReason,
                    request.dto.BankAccountNumber,
                    request.dto.BankName,
                    request.dto.RequestedBy
                );

                // Process Stripe refund asynchronously
               /* _backgroundJobs.Enqueue(() => ProcessStripeRefundAsync(
                    refundId,
                    payment.PaymentIntentId,
                    refundAmount
                ));*/

                // Cancel the booking
                await _bookingRepo.CancelBookingAsync(request.bookingId, request.dto.RequestedBy, refundAmount);

                // Invalidate cache
                await _cache.RemoveAsync($"booking:{request.bookingId}");
                await _cache.RemoveAsync($"booking:summary:{request.bookingId}");

                // Publish refund email
              /*  await _messageQueue.PublishAsync("refund.requested", new
                {
                    BookingId = request.bookingId,
                    RefundId = refundId,
                    RefundReference = refundReference,
                    Email = booking.BookingEmail,
                    RefundAmount = refundAmount
                });*/

                // SignalR notification
                await _hubContext.Clients.Group($"Booking_{request.bookingId}")
                    .SendAsync("RefundRequested", new
                    {
                        BookingId = request.bookingId,
                        RefundAmount = refundAmount,
                        CancellationFee = cancellationFee,
                        RefundReference = refundReference
                    }, cancellationToken);

                var result = new RefundResultDto
                {
                    Success = true,
                    RefundAmount = refundAmount,
                    CancellationFee = cancellationFee,
                    RefundStatus = "Pending",
                    RefundReference = refundReference,
                    ProcessingDays = 7
                };

                return ApiResponse<RefundResultDto>.SuccessResponse(result,
                    $"Refund request submitted. Amount: ${refundAmount:F2}. Processing time: 5-7 business days.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing refund for booking {request.bookingId}");
                return ApiResponse<RefundResultDto>.ErrorResponse("Failed to process refund request", new[] { ex.Message });
            }
        }

        // Background job to process Stripe refund
        public async Task ProcessStripeRefundAsync(int refundId, string paymentIntentId, decimal refundAmount)
        {
            try
            {
                var stripeRefund = await _stripeService.CreateRefundAsync(
                    paymentIntentId,
                    refundAmount,
                    "requested_by_customer"
                );

                await _extendedRepo.ProcessRefundAsync(refundId, stripeRefund.Id, "Completed");

                _logger.LogInformation($"Stripe refund processed: {stripeRefund.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing Stripe refund for refund ID {refundId}");
                await _extendedRepo.ProcessRefundAsync(refundId, null, "Failed");
            }
        }
    }
}
