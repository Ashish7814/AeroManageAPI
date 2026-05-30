using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Weather;
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

namespace AeroManage.FlightManagement.Application.Handlers.Weather
{
    public class GetActiveWeatherAlertsQueryHandler : IRequestHandler<GetActiveWeatherAlertsQuery, ApiResponse<IEnumerable<WeatherAlertDto>>>
    {
        private readonly IWeatherAlertRepository _repository;
        private readonly ICacheService _cache;

        public GetActiveWeatherAlertsQueryHandler(IWeatherAlertRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<ApiResponse<IEnumerable<WeatherAlertDto>>> Handle(GetActiveWeatherAlertsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Cache weather alerts for 5 minutes
                var cacheKey = request.airportId is int airportId
                ? CacheKeys.WeatherAlert(airportId)
                    : "weather:all";

                /*var cacheKey = request.airportId is int airportId
                ? CacheKeys.WeatherAlert(airportId)
                : CacheKeys.WeatherAlertAll;*/

                var cached = await _cache.GetAsync<IEnumerable<WeatherAlertDto>>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<IEnumerable<WeatherAlertDto>>.SuccessResponse(cached);
                }

                var alerts = await _repository.GetActiveAlertsAsync(request.airportId);

                var dtos = alerts.Select(a => new WeatherAlertDto
                {
                    AlertId = a.AlertId,
                    AirportId = a.AirportId,
                    AlertType = a.AlertType,
                    Severity = a.Severity,
                    Description = a.Description,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    IsActive = a.IsActive,
                    AirportCode = a.AirportCode,
                    AirportName = a.AirportName,
                    City = a.City,
                    Country = a.Country
                }).ToList();

                await _cache.SetAsync(cacheKey, dtos, TimeSpan.FromMinutes(5));

                return ApiResponse<IEnumerable<WeatherAlertDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<WeatherAlertDto>>.ErrorResponse("Error fetching weather alerts", new[] { ex.Message });
            }
        }
    }

}
