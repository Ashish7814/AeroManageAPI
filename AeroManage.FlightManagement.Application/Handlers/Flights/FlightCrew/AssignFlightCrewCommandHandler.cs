using AeroManage.FlightManagement.Application.Commands.Flights.FlightCrew;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroMange.Shared.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightCrew
{
    public class AssignFlightCrewCommandHandler : IRequestHandler<AssignFlightCrewCommand, ApiResponse<FlightCrewDto>>
    {
        private readonly IFlightRepository _repository;
        private readonly IAuditLogRepository _auditLog;

        public AssignFlightCrewCommandHandler(IFlightRepository repository, IAuditLogRepository auditLog)
        {
            _repository = repository;
            _auditLog = auditLog;
        }

        public async Task<ApiResponse<FlightCrewDto>> Handle(AssignFlightCrewCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var crew = await _repository.AssignFlightCrewAsync(
                    request.dto.FlightId,
                    request.dto.UserId,
                    request.dto.CrewRole);

                if (crew == null)
                {
                    return ApiResponse<FlightCrewDto>.ErrorResponse("Failed to assign crew");
                }

                await _auditLog.CreateAuditLogAsync(
                    request.dto.UserId,
                    "CREW_ASSIGNED",
                    "FlightCrew",
                    crew.FlightCrewId);

                var dto = new FlightCrewDto
                {
                    FlightCrewId = crew.FlightCrewId,
                    FlightId = crew.FlightId,
                    UserId = crew.UserId,
                    //CrewRole = crew.CrewRole,
                    //AssignedAt = crew.AssignedAt,
                    //FirstName = crew.FirstName,
                    //LastName = crew.LastName,
                    //Email = crew.Email,
                    //PhoneNumber = crew.PhoneNumber
                };

                return ApiResponse<FlightCrewDto>.SuccessResponse(dto, "Crew assigned successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<FlightCrewDto>.ErrorResponse("Failed to assign crew", new[] { ex.Message });
            }
        }
    }
}
