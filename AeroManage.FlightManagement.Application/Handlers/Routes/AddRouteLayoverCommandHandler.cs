using AeroManage.FlightManagement.Application.Commands.Routes;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroMange.Shared.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Routes
{
    public class AddRouteLayoverCommandHandler : IRequestHandler<AddRouteLayoverCommand, ApiResponse<RouteLayoverDto>>
    {
        private readonly IRouteLayoverRepository _routeLayoverRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        public AddRouteLayoverCommandHandler(IRouteLayoverRepository routeLayoverRepository, IAuditLogRepository auditLogRepository)
        {
            _routeLayoverRepository = routeLayoverRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<ApiResponse<RouteLayoverDto>> Handle(AddRouteLayoverCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var layover = await _routeLayoverRepository.AddLayoverAsync(
                    request.dto.RouteId,
                    request.dto.AirportId,
                    request.dto.LayoverSequence,
                    request.dto.MinimumLayoverMinutes,
                    request.dto.MaximumLayoverMinutes
                );

                await _auditLogRepository.CreateAuditLogAsync(null, "LAYOVER_ADDED", "RouteLayovers", layover.LayoverId);

                var dto = new RouteLayoverDto
                {
                    LayoverId = layover.LayoverId,
                    RouteId = layover.RouteId,
                    AirportId = layover.AirportId,
                    LayoverSequence = layover.LayoverSequence,
                    MinimumLayoverMinutes = layover.MinimumLayoverMinutes,
                    MaximumLayoverMinutes = layover.MaximumLayoverMinutes,
                    AirportCode = layover.AirportCode,
                    AirportName = layover.AirportName,
                    City = layover.City
                };

                return ApiResponse<RouteLayoverDto>.SuccessResponse(dto, "Layover added successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<RouteLayoverDto>.ErrorResponse("Failed to add layover", new[] { ex.Message });
            }
        }
    }
}
