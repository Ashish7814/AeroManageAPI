using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Flights;
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

namespace AeroManage.FlightManagement.Application.Handlers.Flights
{
    public class GetFlightDashboardQueryHandler : IRequestHandler<GetFlightDashboardQuery, ApiResponse<IEnumerable<FlightDashboardDto>>>
    {
        private readonly IFlightDashboardRepository _repository;
        private readonly ICacheService _cache;

        public GetFlightDashboardQueryHandler(IFlightDashboardRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<ApiResponse<IEnumerable<FlightDashboardDto>>> Handle(GetFlightDashboardQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Cache dashboard for 1 minute (real-time data)
                var cacheKey = CacheKeys.FlightDashboard(request.dto.AirportId, request.dto.Date);
                var cached = await _cache.GetAsync<IEnumerable<FlightDashboardDto>>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<IEnumerable<FlightDashboardDto>>.SuccessResponse(cached);
                }

                var dashboard = await _repository.GetFlightDashboardAsync(
                    request.dto.AirportId,
                    request.dto.Date,
                    request.dto.Status
                );

                var dtos = dashboard.Select(d => new FlightDashboardDto
                {
                    FlightId = d.FlightId,
                    FlightNumber = d.FlightNumber,
                    FlightStatus = d.FlightStatus,
                    BoardingStatus = d.BoardingStatus,
                    DepartureDateTime = d.DepartureDateTime,
                    ArrivalDateTime = d.ArrivalDateTime,
                    ActualDepartureDateTime = d.ActualDepartureDateTime,
                    ActualArrivalDateTime = d.ActualArrivalDateTime,
                    DelayMinutes = d.DelayMinutes,
                    DepartureGate = d.DepartureGate,
                    ArrivalGate = d.ArrivalGate,
                    RouteCode = d.RouteCode,
                    OriginCode = d.OriginCode,
                    OriginName = d.OriginName,
                    OriginCity = d.OriginCity,
                    DestinationCode = d.DestinationCode,
                    DestinationName = d.DestinationName,
                    DestinationCity = d.DestinationCity,
                    RegistrationNumber = d.RegistrationNumber,
                    AircraftType = d.AircraftType,
                    ActiveNotifications = d.ActiveNotifications,
                    WeatherAlert = d.WeatherAlert
                }).ToList();

                await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(1));

                return ApiResponse<IEnumerable<FlightDashboardDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<FlightDashboardDto>>.ErrorResponse("Error fetching dashboard", new[] { ex.Message });
            }
        }
    }
}
