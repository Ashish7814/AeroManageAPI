using AeroManage.FlightManagement.Application.Commands.Airport;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Domain.Entities;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroMange.Shared.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Airports
{
    public class CreateAirportCommandHandler : IRequestHandler<CreateAirportCommand, ApiResponse<AirportDto>>
    {
        private readonly IAirportRepository _repository;
        private readonly IAuditLogRepository _auditLog;

        public CreateAirportCommandHandler(IAirportRepository repository, IAuditLogRepository auditLog)
        {
            _repository = repository;
            _auditLog = auditLog;
        }

        public async Task<ApiResponse<AirportDto>> Handle(CreateAirportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var airport = new Airport
                {
                    AirportCode = request.dto.AirportCode,
                    ICAOCode = request.dto.ICAOCode,
                    AirportName = request.dto.AirportName,
                    City = request.dto.City,
                    State = request.dto.State,
                    Country = request.dto.Country,
                    Latitude = request.dto.Latitude,
                    Longitude = request.dto.Longitude,
                    Timezone = request.dto.Timezone
                };

                var created = await _repository.CreateAirportAsync(airport);

                await _auditLog.CreateAuditLogAsync(null, "AIRPORT_CREATED", "Airports", created.AirportId);

                var dto = new AirportDto
                {
                    AirportId = created.AirportId,
                    AirportCode = created.AirportCode,
                    ICAOCode = created.ICAOCode,
                    AirportName = created.AirportName,
                    City = created.City,
                    State = created.State,
                    Country = created.Country,
                    Latitude = created.Latitude,
                    Longitude = created.Longitude,
                    Timezone = created.Timezone
                };

                return ApiResponse<AirportDto>.SuccessResponse(dto, "Airport created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<AirportDto>.ErrorResponse("Failed to create airport", new[] { ex.Message });
            }
        }

    }
}
