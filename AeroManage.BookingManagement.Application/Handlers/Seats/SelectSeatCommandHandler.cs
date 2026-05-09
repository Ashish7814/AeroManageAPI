using AeroManage.BookingManagement.Application.Commands.Seats;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Hubs;
using AeroManage.BookingManagement.Domain.Entities;
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
    public class SelectSeatCommandHandler : IRequestHandler<SelectSeatCommand, ApiResponse<SeatReservation>>
    {
        private readonly ISeatRepository _seatRepo;
        private readonly ICacheService _cache;
        private readonly IHubContext<BookingHub> _hubContext;
        private readonly ILogger<SelectSeatCommandHandler> _logger;

        public SelectSeatCommandHandler(
            ISeatRepository seatRepo,
            ICacheService cache,
            IHubContext<BookingHub> hubContext,
            ILogger<SelectSeatCommandHandler> logger)
        {
            _seatRepo = seatRepo;
            _cache = cache;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<ApiResponse<SeatReservation>> Handle(SelectSeatCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get seat ID from seat number
                var seats = await _seatRepo.GetFlightSeatsAsync(request.dto.FlightId);
                var seat = seats.FirstOrDefault(s => s.SeatCode == request.dto.SeatCode);

                if (seat == null)
                {
                    return ApiResponse<SeatReservation>.ErrorResponse("Seat not found");
                }

                var reservation = await _seatRepo.ReserveSeatAsync(
                    request.dto.FlightId,
                    seat.SeatId,
                    request.dto.PassengerId,
                    15 // 15-minute hold
                );

                // Invalidate cache
                await _cache.RemoveAsync($"seat:availability:{request.dto.FlightId}");
                await _cache.RemoveAsync($"flight:seats:{request.dto.FlightId}");

                // SignalR notification
                await _hubContext.Clients.All.SendAsync("SeatReserved", new
                {
                    FlightId = request.dto.FlightId,
                    SeatNumber = request.dto.SeatCode,
                    ExpiresAt = reservation.ExpiresAt
                }, cancellationToken);

                return ApiResponse<SeatReservation>.SuccessResponse(reservation, "Seat reserved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reserving seat");
                return ApiResponse<SeatReservation>.ErrorResponse("Failed to reserve seat", new[] { ex.Message });
            }
        }
    }

}
