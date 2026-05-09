using AeroManage.FlightManagement.Application.Commands.Aircraft;
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

namespace AeroManage.FlightManagement.Application.Handlers.Aircrafts
{
    public class CreateAircraftCommandHandler : IRequestHandler<CreateAircraftCommand, ApiResponse<AircraftDto>>
    {
        private readonly IAircraftRepository _repository;
        private readonly IAuditLogRepository _auditLog;

        public CreateAircraftCommandHandler(IAircraftRepository repository, IAuditLogRepository auditLog)
        {
            _repository = repository;
            _auditLog = auditLog;
        }

        public async Task<ApiResponse<AircraftDto>> Handle(CreateAircraftCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var aircraft = new Aircraft
                {
                    RegistrationNumber = request.dto.RegistrationNumber,
                    AircraftType = request.dto.AircraftType,
                    Manufacturer = request.dto.Manufacturer,
                    Model = request.dto.Model,
                    YearManufactured = request.dto.YearManufactured,
                    //TotalSeats = request.dto.TotalSeats,
                    EconomySeats = request.dto.EconomySeats,
                    BusinessSeats = request.dto.BusinessSeats,
                    FirstClassSeats = request.dto.FirstClassSeats
                };

                var created = await _repository.CreateAircraftAsync(aircraft);

                await _auditLog.CreateAuditLogAsync(null, "AIRCRAFT_CREATED", "Aircraft", created.AircraftId);

                var dto = new AircraftDto
                {
                    AircraftId = created.AircraftId,
                    RegistrationNumber = created.RegistrationNumber,
                    AircraftType = created.AircraftType,
                    Manufacturer = created.Manufacturer,
                    Model = created.Model,
                    YearManufactured = created.YearManufactured,
                    TotalSeats = created.TotalSeats,
                    EconomySeats = created.EconomySeats,
                    BusinessSeats = created.BusinessSeats,
                    FirstClassSeats = created.FirstClassSeats,
                    //CurrentStatus = created.CurrentStatus
                };

                return ApiResponse<AircraftDto>.SuccessResponse(dto, "Aircraft created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<AircraftDto>.ErrorResponse("Failed to create aircraft", new[] { ex.Message });
            }
        }
    }
}
