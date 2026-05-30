using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Mappers;
using AeroManage.FlightManagement.Application.Queries.Flights;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights
{
    public class GetFlightByIdQueryHandler : IRequestHandler<GetFlightByIdQuery, ApiResponse<FlightDto>>
    {
        private readonly IFlightRepository _repository;

        public GetFlightByIdQueryHandler(IFlightRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<FlightDto>> Handle(GetFlightByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var flight = await _repository.GetFlightByIdAsync(request.flightId);

                if (flight == null)
                {
                    return ApiResponse<FlightDto>.ErrorResponse("Flight not found");
                }

                var dto = Mapper.MapToDto(flight);

                return ApiResponse<FlightDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                return ApiResponse<FlightDto>.ErrorResponse("Error fetching flight", new[] { ex.Message });
            }
        }

        
    }

}
