using AeroManage.FlightManagement.Application.Commands.Flights.FlightSchedule;
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

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightSchedule
{
    public class CreateFlightScheduleTemplateCommandHandler : IRequestHandler<CreateFlightScheduleTemplateCommand, ApiResponse<FlightScheduleTemplateDto>>
    {
        private readonly IFlightScheduleTemplateRepository _repository;
        private readonly IAuditLogRepository _auditLog;

        public CreateFlightScheduleTemplateCommandHandler(IFlightScheduleTemplateRepository repository, IAuditLogRepository auditLog)
        {
            _repository = repository;
            _auditLog = auditLog;
        }

        public async Task<ApiResponse<FlightScheduleTemplateDto>> Handle(CreateFlightScheduleTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var template = new FlightScheduleTemplate
                {
                    TemplateName = request.dto.TemplateName,
                    FlightNumberPrefix = request.dto.FlightNumberPrefix,
                    RouteId = request.dto.RouteId,
                    AircraftId = request.dto.AircraftId,
                    //RecurrenceType = request.dto.RecurrenceType,
                    //DaysOfWeek = request.dto.DaysOfWeek,
                    //DayOfMonth = request.dto.DayOfMonth,
                    StartDate = request.dto.StartDate,
                    EndDate = request.dto.EndDate,
                    DepartureTime = request.dto.DepartureTime,
                    ArrivalTime = request.dto.ArrivalTime,
                    //EconomyPrice = request.dto.EconomyPrice,
                    //BusinessPrice = request.dto.BusinessPrice,
                    //FirstClassPrice = request.dto.FirstClassPrice
                };

                var created = await _repository.CreateTemplateAsync(template, request.dto.CreatedBy);

                await _auditLog.CreateAuditLogAsync(request.dto.CreatedBy, "SCHEDULE_TEMPLATE_CREATED", "FlightScheduleTemplates", created.Id);

                var dto = Mapper.MapToTemplateDto(created);

                return ApiResponse<FlightScheduleTemplateDto>.SuccessResponse(dto, "Schedule template created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<FlightScheduleTemplateDto>.ErrorResponse("Failed to create template", new[] { ex.Message });
            }
        }
    }
}
