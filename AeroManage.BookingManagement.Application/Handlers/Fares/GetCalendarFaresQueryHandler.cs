using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Fares;
using AeroManage.BookingManagement.Domain.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Fares
{
    public class GetCalendarFaresQueryHandler : IRequestHandler<GetCalendarFaresQuery, ApiResponse<List<CalendarFareResultDto>>>
    {
        private readonly IBookingRepository _repo;
        private readonly ICalendarFareRepository _calendarFareRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetCalendarFaresQueryHandler> _logger;

        public GetCalendarFaresQueryHandler(
            IBookingRepository repo,
            ICalendarFareRepository calendarFareRepo,
            ICacheService cache,
            ILogger<GetCalendarFaresQueryHandler> logger)
        {
            _repo = repo;
            _calendarFareRepo = calendarFareRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<List<CalendarFareResultDto>>> Handle(GetCalendarFaresQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = $"calendar:{request.dto.OriginAirportId}:{request.dto.DestinationAirportId}:{request.dto.StartDate:yyyyMMdd}";
                var cached = await _cache.GetAsync<List<CalendarFareResultDto>>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<List<CalendarFareResultDto>>.SuccessResponse(cached);
                }

                var calendarFares = await _calendarFareRepo.GetCalendarFaresAsync(
                    request.dto.OriginAirportId,
                    request.dto.DestinationAirportId,
                    request.dto.StartDate,
                    request.dto.EndDate,
                    request.dto.SeatClass
                );

                var result = calendarFares.Select(x => new CalendarFareResultDto
                {
                    RouteId = x.RouteId,
                    FlightDate = x.FlightDate,
                    SeatClass = x.SeatClass,
                    MinPrice = x.MinPrice,
                    AvailableFlights = x.AvailableFlights,
                    LastUpdated = x.LastUpdated
                }).ToList();

                await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(1));

                return ApiResponse<List<CalendarFareResultDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting calendar fares");
                return ApiResponse<List<CalendarFareResultDto>>.ErrorResponse("Failed to retrieve calendar fares", new[] { ex.Message });
            }
        }
    }
}
