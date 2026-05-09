using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Weather;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Weather
{
    public class GetWeatherImpactedFlightsQueryHandler : IRequestHandler<GetWeatherImpactedFlightsQuery, ApiResponse<IEnumerable<FlightDto>>>
    {
        private readonly IWeatherAlertRepository _repository;

        public GetWeatherImpactedFlightsQueryHandler(IWeatherAlertRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<FlightDto>>> Handle(GetWeatherImpactedFlightsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var flights = await _repository.GetWeatherImpactedFlightsAsync(request.alertId);

                var dtos = flights.Select(f => new FlightDto
                {
                    FlightId = f.FlightId,
                    FlightNumber = f.FlightNumber,
                    FlightStatus = f.FlightStatus,
                    DepartureDateTime = f.DepartureDateTime,
                    ArrivalDateTime = f.ArrivalDateTime,
                    //RouteCode = f.RouteCode,
                    //OriginCode = f.OriginCode,
                    //OriginCity = f.OriginCity,
                    //DestinationCode = f.DestinationCode,
                    //DestinationCity = f.DestinationCity
                });

                return ApiResponse<IEnumerable<FlightDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<FlightDto>>.ErrorResponse("Error fetching impacted flights", new[] { ex.Message });
            }
        }
    }
}
