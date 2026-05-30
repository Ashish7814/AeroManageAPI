using AeroManage.FlightManagement.Application.Commands.Flights.FlightGate;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Hubs;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroMange.Shared.Repositories;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightGate
{
    public class AssignFlightGateCommandHandler : IRequestHandler<AssignFlightGateCommand, ApiResponse<GateAssignmentDto>>
    {
        private readonly IGateAssignmentRepository _repository;
        private readonly IAuditLogRepository _auditLog;
        private readonly IHubContext<FlightHub> _hubContext;

        public AssignFlightGateCommandHandler(
            IGateAssignmentRepository repository,
            IAuditLogRepository auditLog,
            IHubContext<FlightHub> hubContext)
        {
            _repository = repository;
            _auditLog = auditLog;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<GateAssignmentDto>> Handle(AssignFlightGateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var assignment = await _repository.AssignGateAsync(
                    request.dto.FlightId,
                    request.dto.GateNumber,
                    request.dto.GateType,
                    request.dto.ScheduledTime,
                    request.dto.AssignedBy
                );

                var dto = new GateAssignmentDto
                {
                    AssignmentId = assignment.AssignmentId,
                    FlightId = assignment.FlightId,
                    GateNumber = assignment.GateNumber,
                    GateType = assignment.GateType,
                    ScheduledTime = assignment.ScheduledTime,
                    ActualTime = assignment.ActualTime,
                    Status = assignment.Status,
                    AssignedAt = assignment.AssignedAt
                };

                await _auditLog.CreateAuditLogAsync(
                    request.dto.AssignedBy,
                    "GATE_ASSIGNED",
                    "GateAssignments",
                    assignment.AssignmentId,
                    null,
                    $"Gate {request.dto.GateNumber} assigned to flight {request.dto.FlightId}"
                );

                // Broadcast gate assignment via SignalR
                await _hubContext.Clients.Group($"Flight_{request.dto.FlightId}")
                    .SendAsync("GateAssigned", new
                    {
                        FlightId = request.dto.FlightId,
                        GateNumber = request.dto.GateNumber,
                        GateType = request.dto.GateType,
                        ScheduledTime = request.dto.ScheduledTime
                    }, cancellationToken);

                return ApiResponse<GateAssignmentDto>.SuccessResponse(dto, "Gate assigned successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<GateAssignmentDto>.ErrorResponse("Failed to assign gate", new[] { ex.Message });
            }
        }
    }
}
