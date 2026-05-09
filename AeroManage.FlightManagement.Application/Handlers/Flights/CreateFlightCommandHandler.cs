using AeroManage.FlightManagement.Application.Commands.Flights;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Mappers;
using AeroManage.FlightManagement.Domain.Entities;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroMange.Shared.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights
{
    public class CreateFlightCommandHandler : IRequestHandler<CreateFlightCommand, ApiResponse<int>>
    {
        private readonly IFlightRepository _repository;
        private readonly IAuditLogRepository _auditLog;

        public CreateFlightCommandHandler(IFlightRepository repository, IAuditLogRepository auditLog)
        {
            _repository = repository;
            _auditLog = auditLog;
        }

        public async Task<ApiResponse<int>> Handle(CreateFlightCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var flight = new Flight
                {
                    FlightNumber = request.dto.FlightNumber,
                    RouteId = request.dto.RouteId,
                    AircraftId = request.dto.AircraftId,
                    DepartureDateTime = request.dto.DepartureDateTime,
                    ArrivalDateTime = request.dto.ArrivalDateTime,
                    FlightStatus = request.dto.FlightStatus,
                    CreatedBy = request.dto.CreatedBy
                    
                };

                var created = await _repository.CreateFlightAsync(flight, request.dto.CreatedBy);

                await _auditLog.CreateAuditLogAsync(request.dto.CreatedBy, "FLIGHT_CREATED", "Flights");

                //var dto = Mapper.MapToDto(created);

                return ApiResponse<int>.SuccessResponse(created, "Flight created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse("Failed to create flight", new[] { ex.Message });
            }
        }
    }
}
