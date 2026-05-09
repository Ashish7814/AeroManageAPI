using AeroManage.FlightManagement.Application.Commands.Flights.FlightStatus;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroMange.Shared.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightStatus
{
    public class UpdateFlightStatusCommandHandler : IRequestHandler<UpdateFlightStatusCommand, ApiResponse<bool>>
    {
        private readonly IFlightRepository _repository;
        private readonly IAuditLogRepository _auditLog;

        public UpdateFlightStatusCommandHandler(IFlightRepository repository, IAuditLogRepository auditLog)
        {
            _repository = repository;
            _auditLog = auditLog;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateFlightStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var updated = await _repository.UpdateFlightStatusAsync(
                    request.dto.FlightId,
                    request.dto.FlightStatus,
                    request.dto.ChangedBy);

                if (updated == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Flight not found");
                }

                await _auditLog.CreateAuditLogAsync(
                    request.dto.ChangedBy,
                    "FLIGHT_STATUS_UPDATED",
                    "Flights",
                    updated.FlightId);

                var dto = new FlightDto
                {
                    FlightId = updated.FlightId,
                    FlightNumber = updated.FlightNumber,
                    FlightStatus = updated.FlightStatus,
                    DepartureDateTime = updated.DepartureDateTime,
                    ArrivalDateTime = updated.ArrivalDateTime
                };

                return ApiResponse<bool>.SuccessResponse(true, "Flight status updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse("Failed to update flight status", new[] { ex.Message });
            }
        }
    }
}
