using AeroManage.FlightManagement.Application.Commands.Flights.FlightSchedule;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroMange.Shared.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightSchedule
{
    public class DeactivateScheduleTemplateCommandHandler : IRequestHandler<DeactivateScheduleTemplateCommand, ApiResponse<bool>>
    {
        private readonly IFlightScheduleTemplateRepository _repository;
        private readonly IAuditLogRepository _auditLog;

        public DeactivateScheduleTemplateCommandHandler(
            IFlightScheduleTemplateRepository repository,
            IAuditLogRepository auditLog)
        {
            _repository = repository;
            _auditLog = auditLog;
        }

        public async Task<ApiResponse<bool>> Handle(DeactivateScheduleTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repository.DeactivateTemplateAsync(request.TemplateId);

                if (!result)
                {
                    return ApiResponse<bool>.ErrorResponse("Template not found or already deactivated");
                }

                await _auditLog.CreateAuditLogAsync(
                    null,
                    "SCHEDULE_TEMPLATE_DEACTIVATED",
                    "FlightScheduleTemplates",
                    request.TemplateId
                );

                return ApiResponse<bool>.SuccessResponse(true, "Schedule template deactivated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse("Failed to deactivate template", new[] { ex.Message });
            }
        }
    }
}
