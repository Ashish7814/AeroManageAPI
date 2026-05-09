using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Airports;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Airports
{
    public class GetAirportByIdQueryHandler : IRequestHandler<GetAirportByIdQuery, ApiResponse<AirportDto>>
    {
        private readonly IAirportRepository _repository;

        public GetAirportByIdQueryHandler(IAirportRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<AirportDto>> Handle(GetAirportByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var airport = await _repository.GetAirportByIdAsync(request.AirportId);

                if (airport == null)
                {
                    return ApiResponse<AirportDto>.ErrorResponse("Airport not found");
                }

                var dto = new AirportDto
                {
                    AirportId = airport.AirportId,
                    AirportCode = airport.AirportCode,
                    ICAOCode = airport.ICAOCode,
                    AirportName = airport.AirportName,
                    City = airport.City,
                    State = airport.State,
                    Country = airport.Country,
                    Latitude = airport.Latitude,
                    Longitude = airport.Longitude,
                    Timezone = airport.Timezone
                };

                return ApiResponse<AirportDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                return ApiResponse<AirportDto>.ErrorResponse("Error fetching airport", new[] { ex.Message });
            }
        }
    }
}
