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
    public class ReleaseSeatReservationCommandHandler: IRequestHandler<ReleaseSeatReservationCommand, ApiResponse<bool>>
    {
        private readonly ISeatRepository _seatRepo;
        private readonly ICacheService _cache;
        private readonly IHubContext<BookingHub> _hubContext;
        private readonly ILogger<ReleaseSeatReservationCommandHandler> _logger;

        public ReleaseSeatReservationCommandHandler(
            ISeatRepository seatRepo,
            ICacheService cache,
            IHubContext<BookingHub> hubContext,
            ILogger<ReleaseSeatReservationCommandHandler> logger)
        {
            _seatRepo = seatRepo;
            _cache = cache;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<ApiResponse<bool>> Handle(
            ReleaseSeatReservationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var released = await _seatRepo.ReleaseSeatReservationAsync(request.reservationId);
                if (!released)
                    return ApiResponse<bool>.ErrorResponse(
                        "Reservation not found or already released");

                // Invalidate seat cache so new availability is reflected
                await _cache.RemoveAsync($"seat:map:{request.flightId}");
                await _cache.RemoveAsync($"seat:available:{request.flightId}:Economy");
                await _cache.RemoveAsync($"seat:available:{request.flightId}:Business");
                await _cache.RemoveAsync($"seat:available:{request.flightId}:FirstClass");

                // Broadcast real-time availability update
                await _hubContext.Clients.All.SendAsync(
                    "SeatAvailabilityChanged",
                    new { FlightId = request.flightId, ReservationId = request.reservationId },
                    cancellationToken);

                _logger.LogInformation(
                    $"Released reservation {request.reservationId} on flight {request.flightId}");

                return ApiResponse<bool>.SuccessResponse(true, "Seat reservation released");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error releasing reservation {request.reservationId}");
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to release reservation", new[] { ex.Message });
            }
        }
    }

}
