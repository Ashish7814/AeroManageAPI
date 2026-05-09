using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Routes;
using AeroManage.FlightManagement.Application.Services.Implementation;
using AeroManage.FlightManagement.Application.Services.Interfaces;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Implementation;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Routes
{
    public class GetRouteLayoversQueryHandler : IRequestHandler<GetRouteLayoversQuery, ApiResponse<IEnumerable<RouteLayoverDto>>>
    {
        private readonly IRouteLayoverRepository _repository;
        private readonly ICacheService _cache;

        public GetRouteLayoversQueryHandler(IRouteLayoverRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<ApiResponse<IEnumerable<RouteLayoverDto>>> Handle(GetRouteLayoversQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Try cache first
                var cacheKey = CacheKeys.Route(request.routeId) + ":layovers";
                var cached = await _cache.GetAsync<IEnumerable<RouteLayoverDto>>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<IEnumerable<RouteLayoverDto>>.SuccessResponse(cached);
                }

                var layovers = await _repository.GetRouteLayoversAsync(request.routeId);

                var dtos = layovers.Select(l => new RouteLayoverDto
                {
                    LayoverId = l.LayoverId,
                    RouteId = l.RouteId,
                    AirportId = l.AirportId,
                    LayoverSequence = l.LayoverSequence,
                    MinimumLayoverMinutes = l.MinimumLayoverMinutes,
                    MaximumLayoverMinutes = l.MaximumLayoverMinutes,
                    AirportCode = l.AirportCode,
                    AirportName = l.AirportName,
                    City = l.City,
                    Country = l.Country
                }).ToList();

                // Cache for 30 minutes
                await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(30));

                return ApiResponse<IEnumerable<RouteLayoverDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RouteLayoverDto>>.ErrorResponse("Error fetching layovers", new[] { ex.Message });
            }
        }
    }
}
