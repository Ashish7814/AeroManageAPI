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
    public class GetRouteWithLayoversQueryHandler : IRequestHandler<GetRouteWithLayoversQuery, ApiResponse<RouteWithLayoversDto>>
    {
        private readonly IRouteLayoverRepository _layoverRepository;
        private readonly IRouteRepository _routeRepository;
        private readonly ICacheService _cache;

        public GetRouteWithLayoversQueryHandler(
            IRouteLayoverRepository layoverRepository,
            IRouteRepository routeRepository,
            ICacheService cache)
        {
            _layoverRepository = layoverRepository;
            _routeRepository = routeRepository;
            _cache = cache;
        }

        public async Task<ApiResponse<RouteWithLayoversDto>> Handle(GetRouteWithLayoversQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Try cache first
                var cacheKey = CacheKeys.Route(request.routedId) + ":with-layovers";
                var cached = await _cache.GetAsync<RouteWithLayoversDto>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<RouteWithLayoversDto>.SuccessResponse(cached);
                }

                // Get route details
                var route = await _routeRepository.GetRouteByIdAsync(request.routedId);

                if (route == null)
                {
                    return ApiResponse<RouteWithLayoversDto>.ErrorResponse("Route not found");
                }

                // Get layovers
                var layovers = await _layoverRepository.GetRouteLayoversAsync(request.routedId);

                // Map to DTOs
                var routeDto = new RouteDto
                {
                    RouteId = route.RouteId,
                    RouteCode = route.RouteCode,
                    OriginAirportId = route.OriginAirportId,
                    DestinationAirportId = route.DestinationAirportId,
                    Distance = route.Distance,
                    EstimatedDuration = route.EstimatedDuration,
                    //OriginCode = route.OriginCode,
                    //OriginName = route.OriginName,
                    //OriginCity = route.OriginCity,
                    //OriginCountry = route.OriginCountry,
                    //DestinationCode = route.DestinationCode,
                    //DestinationName = route.DestinationName,
                    //DestinationCity = route.DestinationCity,
                    //DestinationCountry = route.DestinationCountry
                };

                var layoverDtos = layovers.Select(l => new RouteLayoverDto
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
                }).OrderBy(l => l.LayoverSequence).ToArray();

                var result = new RouteWithLayoversDto
                {
                    Route = routeDto,
                    Layovers = layoverDtos
                };

                // Cache for 30 minutes
                await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));

                return ApiResponse<RouteWithLayoversDto>.SuccessResponse(result, $"Route with {layoverDtos.Length} layover(s)");
            }
            catch (Exception ex)
            {
                return ApiResponse<RouteWithLayoversDto>.ErrorResponse("Error fetching route with layovers", new[] { ex.Message });
            }
        }
    }
}
