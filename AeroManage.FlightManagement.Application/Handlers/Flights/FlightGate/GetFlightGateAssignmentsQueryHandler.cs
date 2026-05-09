using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Flights.FlightGate;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightGate
{
    public class GetFlightGateAssignmentsQueryHandler : IRequestHandler<GetFlightGateAssignmentsQuery, ApiResponse<IEnumerable<GateAssignmentDto>>>
    {
        private readonly IGateAssignmentRepository _repository;

        public GetFlightGateAssignmentsQueryHandler(IGateAssignmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<GateAssignmentDto>>> Handle(GetFlightGateAssignmentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var assignments = await _repository.GetFlightGateAssignmentsAsync(request.FlightId);

                var dtos = assignments.Select(a => new GateAssignmentDto
                {
                    AssignmentId = a.AssignmentId,
                    FlightId = a.FlightId,
                    GateNumber = a.GateNumber,
                    GateType = a.GateType,
                    ScheduledTime = a.ScheduledTime,
                    ActualTime = a.ActualTime,
                    Status = a.Status,
                    AssignedAt = a.AssignedAt
                });

                return ApiResponse<IEnumerable<GateAssignmentDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<GateAssignmentDto>>.ErrorResponse("Error fetching gate assignments", new[] { ex.Message });
            }
        }
    }
}

