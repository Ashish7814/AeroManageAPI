using AeroManage.BookingManagement.Application.Commands.Seats;
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

namespace AeroManage.BookingManagement.Application.Handlers.Seats
{
    public class ChangeSeatCommandHandler : IRequestHandler<ChangeSeatCommand, ApiResponse<bool>>
    {
        private readonly IBookingRepository _repo;
        private readonly ISeatRepository _seatRepo;
        private readonly ICacheService _cache;
        private readonly IHubContext<BookingHub> _hubContext;
        /*private readonly IMessageQueueService _messageQueue;*/
        private readonly ILogger<ChangeSeatCommandHandler> _logger;

        public ChangeSeatCommandHandler(
            IBookingRepository repo,
            ISeatRepository seatRepo,
            ICacheService cache,
            IHubContext<BookingHub> hubContext,
           /* IMessageQueueService messageQueue,*/
            ILogger<ChangeSeatCommandHandler> logger)
        {
            _repo = repo;
            _seatRepo = seatRepo;
            _cache = cache;
            _hubContext = hubContext;
         /*   _messageQueue = messageQueue;*/
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(ChangeSeatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Change seat
                var result = await _repo.ChangeSeatAsync(
                    request.dto.PassengerId,
                    request.dto.FlightId,
                    request.dto.NewSeatNumber,
                    request.dto.ChangedBy
                );

                if (!result)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to change seat. Seat may not be available.");
                }

                // Invalidate cache
                await _cache.RemoveAsync($"booking:{request.bookingId}");
                await _cache.RemoveAsync($"booking:summary:{request.bookingId}");
                await _cache.RemoveAsync($"seat:availability:{request.dto.FlightId}");
                await _cache.RemoveAsync($"flight:seats:{request.dto.FlightId}");

                // Publish notification
               /* await _messageQueue.PublishAsync("booking.modified", new
                {
                    BookingId = request.bookingId,
                    ModificationType = "SeatChange",
                    NewSeatNumber = request.dto.NewSeatNumber
                });*/

                // SignalR notification
                await _hubContext.Clients.Group($"Booking_{request.bookingId}")
                    .SendAsync("SeatChanged", new
                    {
                        BookingId = request.bookingId,
                        PassengerId = request.dto.PassengerId,
                        NewSeatNumber = request.dto.NewSeatNumber,
                        Message = "Seat changed successfully"
                    }, cancellationToken);

                // Broadcast seat availability update to all users viewing this flight
                await _hubContext.Clients.All.SendAsync("SeatAvailabilityChanged", new
                {
                    FlightId = request.dto.FlightId
                }, cancellationToken);

                return ApiResponse<bool>.SuccessResponse(true, $"Seat changed to {request.dto.NewSeatNumber} successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing seat for passenger {request.dto.PassengerId}");
                return ApiResponse<bool>.ErrorResponse("Failed to change seat", new[] { ex.Message });
            }
        }
    }
}
