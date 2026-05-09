using AeroManage.FlightManagement.Application.Commands.Aircraft;
using AeroManage.FlightManagement.Application.DTOs;
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
    public class UpdateAircraftStatusCommandHandler : IRequestHandler<UpdateAircraftStatusCommand, ApiResponse<AircraftDto>>
    {
        private readonly IAircraftRepository _repository;
        private readonly IAuditLogRepository _auditLog;

        public UpdateAircraftStatusCommandHandler(IAircraftRepository repository, IAuditLogRepository auditLog)
        {
            _repository = repository;
            _auditLog = auditLog;
        }

        public async Task<ApiResponse<AircraftDto>> Handle(UpdateAircraftStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var updated = await _repository.UpdateAircraftStatusAsync(request.dto.AircraftId, request.dto.Status);

                if (updated == null)
                {
                    return ApiResponse<AircraftDto>.ErrorResponse("Aircraft not found");
                }

                await _auditLog.CreateAuditLogAsync(null, "AIRCRAFT_STATUS_UPDATED", "Aircraft", updated.AircraftId);

                var dto = new AircraftDto
                {
                    AircraftId = updated.AircraftId,
                    RegistrationNumber = updated.RegistrationNumber,
                    AircraftType = updated.AircraftType,
                    Manufacturer = updated.Manufacturer,
                    Model = updated.Model,
                    YearManufactured = updated.YearManufactured,
                    TotalSeats = updated.TotalSeats,
                    EconomySeats = updated.EconomySeats,
                    BusinessSeats = updated.BusinessSeats,
                    FirstClassSeats = updated.FirstClassSeats,
                    //CurrentStatus = updated.CurrentStatus,
                    LastMaintenanceDate = updated.LastMaintenanceDate,
                    NextMaintenanceDate = updated.NextMaintenanceDate
                };

                return ApiResponse<AircraftDto>.SuccessResponse(dto, "Aircraft status updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<AircraftDto>.ErrorResponse("Failed to update aircraft status", new[] { ex.Message });
            }
        }
    }
}
