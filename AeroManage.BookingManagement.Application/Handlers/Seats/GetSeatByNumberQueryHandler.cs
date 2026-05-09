using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Seats;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Seats
{
    public class GetSeatByNumberQueryHandler : IRequestHandler<GetSeatByNumberQuery, ApiResponse<SeatDto>>
    {
        private readonly ISeatRepository _seatRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetSeatByNumberQueryHandler> _logger;

        public GetSeatByNumberQueryHandler(
            ISeatRepository seatRepo,
            ICacheService cache,
            ILogger<GetSeatByNumberQueryHandler> logger)
        {
            _seatRepo = seatRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<SeatDto>> Handle(GetSeatByNumberQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"seat:number:{request.flightId}:{request.seatNumber}";
                var cached = await _cache.GetAsync<SeatDto>(cacheKey);
                if (cached != null)
                    return ApiResponse<SeatDto>.SuccessResponse(cached);

                var seat = await _seatRepo.GetSeatByNumberAsync(
                    request.flightId, request.seatNumber);

                if (seat == null)
                    return ApiResponse<SeatDto>.ErrorResponse(
                        $"Seat {request.seatNumber} not found on flight {request.flightId}");

                var dto = new SeatDto
                {
                      SeatId = seat.SeatId,
                        AircraftId = seat.AircraftId,
                        SeatCode = seat.SeatCode,
                        RowNumber = seat.RowNumber,
                        Column  = seat.Column,
                        IsOccupied = seat.IsOccupied,
                        //IsAvailable = seat.IsAvailable,
                        IsAisle = seat.IsAisle,
                        IsExitRow = seat.IsExitRow,
                        ExtraLegroom = seat.ExtraLegroom,
                        Price = seat.Price,
                        IsSelected = seat.IsSelected,
                        availabilityDto = new SeatAvailabilityDto
                        {
                            seatClass = seat.availabilityDto.SeatClass,
                            AvailableSeats = seat.availabilityDto.AvailableSeats,
                            BusinessAvailable = seat.availabilityDto.BusinessAvailable,
                            FirstClassAvailable = seat.availabilityDto.FirstClassAvailable,
                            EconomyAvailable = seat.availabilityDto.EconomyAvailable,
                            TotalAvailable = seat.availabilityDto.TotalSeats
                        }
                };

                // Cache for 5 min — availability can change quickly
                await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));

                return ApiResponse<SeatDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting seat {request.seatNumber} on flight {request.flightId}");
                return ApiResponse<SeatDto>.ErrorResponse("Failed to retrieve seat", new[] { ex.Message });
            }
        }
    }
}
