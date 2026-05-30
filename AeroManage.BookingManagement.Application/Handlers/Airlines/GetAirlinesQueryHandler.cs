using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Airlines;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Airlines
{
    public class GetAirlinesQueryHandler : IRequestHandler<GetAirlinesQuery, ApiResponse<List<AirlineDto>>>
    {
        private readonly IBookingRepository _repo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetAirlinesQueryHandler> _logger;

        public GetAirlinesQueryHandler(
            IBookingRepository repo,
            ICacheService cache,
            ILogger<GetAirlinesQueryHandler> logger)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ApiResponse<List<AirlineDto>>> Handle(GetAirlinesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = "airlines:active";
                var cached = await _cache.GetAsync<List<AirlineDto>>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<List<AirlineDto>>.SuccessResponse(cached);
                }

                var activeAirline = await _repo.GetActiveAirlinesAsync();

                var result = activeAirline.Select(a => new AirlineDto
                {
                    AirlineId = a.AirlineId,
                    AirlineCode = a.AirlineCode,
                    AirlineName = a.AirlineName,
                    Country = a.Country,
                    LogoUrl = a.LogoUrl,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                }).ToList();

                await _cache.SetAsync(cacheKey, result, TimeSpan.FromHours(24));

                return ApiResponse<List<AirlineDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting airlines");
                return ApiResponse<List<AirlineDto>>.ErrorResponse("Failed to retrieve airlines", new[] { ex.Message });
            }
        }
    }
}
