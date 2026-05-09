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
    public class GetSeatMapQueryHandler : IRequestHandler<GetSeatMapQuery, ApiResponse<List<SeatMapDto>>>
    {
        private readonly ISeatRepository _seatRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetSeatMapQueryHandler> _logger;

        public GetSeatMapQueryHandler(
            ISeatRepository seatRepo,
            ICacheService cache,
            ILogger<GetSeatMapQueryHandler> logger)
        {
            _seatRepo = seatRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<List<SeatMapDto>>> Handle(GetSeatMapQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"seat:map:{request.flightId}";
                var cached = await _cache.GetAsync<List<SeatMapDto>>(cacheKey);
                if (cached != null)
                    return ApiResponse<List<SeatMapDto>>.SuccessResponse(cached);

                var seatMap = await _seatRepo.GetSeatMapAsync(request.flightId);
                var result = new SeatMapDto
                {
                    FlightId = request.flightId, // assign actual value
                    Seatdto = seatMap.Select(s => new SeatDto
                    {
                        SeatId = s.SeatId,
                        AircraftId = s.AircraftId,
                        SeatCode = s.SeatCode,
                        RowNumber = s.RowNumber,
                        Column = s.Column,
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
                    }).ToList()
                };

                // Cache for 2 min — live availability
                await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(2));

                return ApiResponse<List<SeatMapDto>>.SuccessResponse(new List<SeatMapDto> { result },$"Seat map loaded: {result.Seatdto.Count} seats");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting seat map for flight {request.flightId}");
                return ApiResponse<List<SeatMapDto>>.ErrorResponse(
                    "Failed to retrieve seat map", new[] { ex.Message });
            }
        }
    }
}
