using AeroManage.BookingManagement.Application.Commands.Bookings;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Hubs;
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

namespace AeroManage.BookingManagement.Application.Handlers.Bookings
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, ApiResponse<bool>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IStripePaymentService _stripeService;
        private readonly IBookingRepository _extendedRepo;
        private readonly ICacheService _cache;
        private readonly IHubContext<BookingHub> _hubContext;
        /*private readonly IMessageQueueService _messageQueue;*/
        private readonly ILogger<CancelBookingCommandHandler> _logger;

        public CancelBookingCommandHandler(
            IBookingRepository bookingRepo,
            IPaymentRepository paymentRepo,
            IStripePaymentService stripeService,
            IBookingRepository extendedRepo,
            ICacheService cache,
            IHubContext<BookingHub> hubContext,
            /*IMessageQueueService messageQueue,*/
            ILogger<CancelBookingCommandHandler> logger)
        {
            _bookingRepo = bookingRepo;
            _paymentRepo = paymentRepo;
            _stripeService = stripeService;
            _extendedRepo = extendedRepo;
            _cache = cache;
            _hubContext = hubContext;
            /*_messageQueue = messageQueue;*/
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get booking using repository
                var booking = await _bookingRepo.GetBookingByIdAsync(request.bookingId);

                if (booking == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Booking not found");
                }

                if (booking.BookingStatus == "Cancelled")
                {
                    return ApiResponse<bool>.ErrorResponse("Booking already cancelled");
                }

                // Calculate refund amount
                var cancellationFee = await _extendedRepo.CalculateCancellationFeeAsync(request.bookingId);
                var refundAmount = booking.TotalAmount - cancellationFee;

                // Process refund if payment was made
                if (booking.PaymentStatus == "Paid" && !string.IsNullOrEmpty(booking.PaymentIntentId))
                {
                    var payment = await _paymentRepo.GetPaymentByBookingIdAsync(request.bookingId);

                    if (payment != null && refundAmount > 0)
                    {
                        await _stripeService.CreateRefundAsync(
                            booking.PaymentIntentId,
                            refundAmount,
                            "requested_by_customer"
                        );

                        await _paymentRepo.ProcessRefundAsync(payment.PaymentId, refundAmount, DateTime.UtcNow);
                    }
                }

                // Cancel booking using repository
                await _bookingRepo.CancelBookingAsync(request.bookingId, request.dto.RequestBy, refundAmount);

                // Invalidate cache
                await _cache.RemoveAsync($"booking:{request.bookingId}");
                await _cache.RemoveAsync($"booking:summary:{request.bookingId}");

                // Publish cancellation email
                /*await _messageQueue.PublishAsync("booking.cancelled", new
                {
                    BookingId = request.bookingId,
                    BookingReference = booking.BookingReference,
                    Email = booking.BookingEmail,
                    RefundAmount = refundAmount,
                    CancellationFee = cancellationFee
                });*/

                // SignalR notification
                await _hubContext.Clients.Group($"Booking_{request.bookingId}")
                    .SendAsync("BookingCancelled", new
                    {
                        BookingId = request.bookingId,
                        RefundAmount = refundAmount
                    }, cancellationToken);

                return ApiResponse<bool>.SuccessResponse(true,
                    $"Booking cancelled successfully. Refund of ${refundAmount:F2} will be processed within 5-7 business days.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling booking {request.bookingId}");
                return ApiResponse<bool>.ErrorResponse("Failed to cancel booking", new[] { ex.Message });
            }
        }
    }

}
