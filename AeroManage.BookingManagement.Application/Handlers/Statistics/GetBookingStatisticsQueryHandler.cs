using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Statistics;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Statistics
{
    public class GetBookingStatisticsQueryHandler : IRequestHandler<GetBookingStatisticsQuery, BookingStatisticsDto>
    {
        private readonly IBookingPricingRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetBookingStatisticsQueryHandler> _logger;

        public GetBookingStatisticsQueryHandler(
            IBookingPricingRepository repository,
            ICacheService cacheService,
            ILogger<GetBookingStatisticsQueryHandler> logger)
        {
            _repository = repository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<BookingStatisticsDto> Handle(GetBookingStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving booking statistics for user {UserId}", request.userId);

                // Generate cache key
                var cacheKey = $"BookingStats:{request.userId}:{request.fromDate?.ToString("yyyyMMdd")}:{request.toDate?.ToString("yyyyMMdd")}";

                // Try get from cache (valid for 10 minutes)
                var cached = await _cacheService.GetAsync<BookingStatisticsDto>(cacheKey);
                if (cached != null)
                {
                    _logger.LogInformation("Statistics served from cache");
                    return cached;
                }

                // Get fresh statistics
                /*  var stats = await _repository.GetBookingStatisticsAsync(request.userId, request.fromDate, request.toDate);

                  var statsDto = new BookingStatisticsDto
                  {
                      TotalBookings = stats.TotalBookings,
                      ActiveBookings = stats.ActiveBookings,
                      CancelledBookings = stats.CancelledBookings,
                      CompletedBookings = stats.CompletedBookings,
                      TotalRevenue = stats.TotalRevenue,
                      AverageBookingValue = stats.AverageBookingValue,
                      TotalPassengers = stats.TotalPassengers,
                      BookingsByStatus = new Dictionary<string, int>(stats.BookingsByStatus),
                      BookingsByMonth = new Dictionary<string, int>(stats.BookingsByMonth)
                  };

                  // Cache for 10 minutes
                  await _cacheService.SetAsync(cacheKey, statsDto, TimeSpan.FromMinutes(10));

                  _logger.LogInformation("Statistics retrieved: {TotalBookings} bookings, ${TotalRevenue} revenue",
                      statsDto.TotalBookings,
                      statsDto.TotalRevenue);

                  return statsDto;*/

                var statsDto = new BookingStatisticsDto
                {
                    TotalBookings = 42,
                    ActiveBookings = 18,
                    CancelledBookings = 5,
                    CompletedBookings = 19,
                    TotalRevenue = 12500.75m,
                    AverageBookingValue = 297.63m,
                    TotalPassengers = 68,
                    BookingsByStatus = new Dictionary<string, int>
            {
                { "Confirmed", 18 },
                { "Pending", 5 },
                { "Cancelled", 5 },
                { "Completed", 14 },
                { "Expired", 0 }
            },
                    BookingsByMonth = new Dictionary<string, int>
            {
                { "Jan", 5 },
                { "Feb", 8 },
                { "Mar", 12 },
                { "Apr", 7 },
                { "May", 10 }
            }
                };

                _logger.LogInformation("Returning static statistics: {TotalBookings} bookings, ${TotalRevenue} revenue",
                    statsDto.TotalBookings,
                    statsDto.TotalRevenue);

                return await Task.FromResult(statsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking statistics");
                throw;
            }
        }
    }
}
