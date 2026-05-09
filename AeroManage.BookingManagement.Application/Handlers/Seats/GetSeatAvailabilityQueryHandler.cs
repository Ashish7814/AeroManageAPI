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
    public class GetSeatAvailabilityQueryHandler : IRequestHandler<GetSeatAvailabilityQuery, ApiResponse<SeatAvailabilityDto>>
    {
        private readonly IBookingRepository _repo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetSeatAvailabilityQueryHandler> _logger;

        public GetSeatAvailabilityQueryHandler(
            IBookingRepository repo,
            ICacheService cache,
            ILogger<GetSeatAvailabilityQueryHandler> logger)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<SeatAvailabilityDto>> Handle(GetSeatAvailabilityQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"seat:availability:{request.flightId}:{request.seatClass}";
                var cached = await _cache.GetAsync<SeatAvailabilityDto>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<SeatAvailabilityDto>.SuccessResponse(cached);
                }

                var seatAvailability = await _repo.GetSeatAvailabilityAsync(request.flightId, request.seatClass);

                var result = new SeatAvailabilityDto
                {
                    seatClass = seatAvailability.SeatClass,
                    TotalAvailable = seatAvailability.TotalSeats,
                    EconomyAvailable = seatAvailability.EconomyAvailable,
                    BusinessAvailable = seatAvailability.BusinessAvailable,
                    FirstClassAvailable = seatAvailability.FirstClassAvailable,
                    AvailableSeats = seatAvailability.AvailableSeats,
                };

                await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

                return ApiResponse<SeatAvailabilityDto>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting seat availability for flight {request.flightId}");
                return ApiResponse<SeatAvailabilityDto>.ErrorResponse("Failed to check seat availability", new[] { ex.Message });
            }
        }
    }
}
