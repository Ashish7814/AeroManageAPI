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
    public class GetAvailableSeatsByClassQueryHandler : IRequestHandler<GetAvailableSeatsByClassQuery, ApiResponse<List<SeatDto>>>
    {
        private readonly ISeatRepository _seatRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetAvailableSeatsByClassQueryHandler> _logger;

        public GetAvailableSeatsByClassQueryHandler(
            ISeatRepository seatRepo,
            ICacheService cache,
            ILogger<GetAvailableSeatsByClassQueryHandler> logger)
        {
            _seatRepo = seatRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<List<SeatDto>>> Handle(GetAvailableSeatsByClassQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"seat:available:{request.flightId}:{request.seatClass}";
                var cached = await _cache.GetAsync<List<SeatDto>>(cacheKey);
                if (cached != null)
                    return ApiResponse<List<SeatDto>>.SuccessResponse(cached);

                var seats = await _seatRepo.GetAvailableSeatsByClassAsync(
                    request.flightId, request.seatClass);

                var dtos = new List<SeatDto>();
                foreach (var s in seats)
                {
                    dtos.Add(new SeatDto
                    {
                        SeatId = s.SeatId,
                        AircraftId = s.AircraftId,
                        SeatCode = s.SeatCode,
                        RowNumber = s.RowNumber,
                        Column  = s.Column,
                        IsOccupied = s.IsOccupied,
                        //IsAvailable = s.IsAvailable,
                        IsAisle = s.IsAisle,
                        IsExitRow = s.IsExitRow,
                        ExtraLegroom = s.ExtraLegroom,
                        Price = s.Price,
                        IsSelected = s.IsSelected,
                        availabilityDto = new SeatAvailabilityDto
                        {
                            seatClass = s.availabilityDto.SeatClass,
                            AvailableSeats = s.availabilityDto.AvailableSeats,
                            BusinessAvailable = s.availabilityDto.BusinessAvailable,
                            FirstClassAvailable = s.availabilityDto.FirstClassAvailable,
                            EconomyAvailable = s.availabilityDto.EconomyAvailable,
                            TotalAvailable = s.availabilityDto.TotalSeats
                        }
                    });
                }

                // Cache for 3 min — availability changes frequently
                await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(3));

                return ApiResponse<List<SeatDto>>.SuccessResponse(
                    dtos, $"{dtos.Count} available {request.seatClass} seats");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting available seats for flight {request.flightId}");
                return ApiResponse<List<SeatDto>>.ErrorResponse(
                    "Failed to retrieve available seats", new[] { ex.Message });
            }
        }
    }
}
