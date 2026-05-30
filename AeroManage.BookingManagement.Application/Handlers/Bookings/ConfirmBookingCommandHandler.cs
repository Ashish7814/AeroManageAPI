using AeroManage.BookingManagement.Application.Commands.Bookings;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Hubs;
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
    public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, ApiResponse<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly ISeatRepository _seatRepo;
        private readonly ICacheService _cache;
        private readonly IHubContext<BookingHub> _hubContext;
   /*     private readonly IMessageQueueService _messageQueue;
        private readonly Hangfire.IBackgroundJobClient _backgroundJobs;*/
        private readonly ILogger<ConfirmBookingCommandHandler> _logger;

        public ConfirmBookingCommandHandler(
            IBookingRepository bookingRepo,
            ISeatRepository seatRepo,
            ICacheService cache,
            IHubContext<BookingHub> hubContext,
            /*IMessageQueueService messageQueue,
            Hangfire.IBackgroundJobClient backgroundJobs,*/
            ILogger<ConfirmBookingCommandHandler> logger)
        {
            _bookingRepo = bookingRepo;
            _seatRepo = seatRepo;
            _cache = cache;
            _hubContext = hubContext;
            /*_messageQueue = messageQueue;
            _backgroundJobs = backgroundJobs;*/
            _logger = logger;
        }

        public async Task<ApiResponse<BookingDto>> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Confirm booking using repository
                var booking = await _bookingRepo.ConfirmBookingAsync(request.dto.BookingId, request.dto.PaymentIntentId);

                if (booking == null)
                {
                    return ApiResponse<BookingDto>.ErrorResponse("Booking not found");
                }

                // Confirm seat reservations
                await _seatRepo.ConfirmSeatReservationsAsync(request.dto.BookingId);

                // Invalidate cache
                await _cache.RemoveAsync($"booking:{request.dto.BookingId}");
                await _cache.RemoveAsync($"booking:summary:{request.dto.BookingId}");

                // Enqueue e-ticket generation (background job)
                /*_backgroundJobs.Enqueue<IBookingBackgroundService>(
                    service => service.GenerateETicketsAsync(request.BookingId)
                );*/

                // Publish to RabbitMQ for email notification
                /*await _messageQueue.PublishAsync("booking.confirmed", new
                {
                    BookingId = request.dto.BookingId,
                    BookingReference = booking.BookingReference,
                    Email = booking.BookingEmail
                });*/

                // SignalR broadcast
                await _hubContext.Clients.Group($"Booking_{request.dto.BookingId}")
                    .SendAsync("BookingConfirmed", new
                    {
                        BookingId = request.dto.BookingId,
                        BookingReference = booking.BookingReference,
                        PNR = booking.PNR,
                        Status = "Confirmed"
                    }, cancellationToken);

                var dto = new BookingDto
                {
                    BookingId = booking.BookingId,
                    BookingReference = booking.BookingReference,
                    PNR = booking.PNR,
                    BookingStatus = "Confirmed",
                    PaymentStatus = "Paid",
                    TotalAmount = booking.TotalAmount,
                    Currency = booking.Currency
                };

                return ApiResponse<BookingDto>.SuccessResponse(dto, "Booking confirmed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error confirming booking {request.dto.BookingId}");
                return ApiResponse<BookingDto>.ErrorResponse("Failed to confirm booking", new[] { ex.Message });
            }
        }
    }
}
