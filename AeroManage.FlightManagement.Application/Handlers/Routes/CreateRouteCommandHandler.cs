using AeroManage.FlightManagement.Application.Commands.Routes;
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

namespace AeroManage.FlightManagement.Application.Handlers.Routes
{
    internal class CreateRouteCommandHandler : IRequestHandler<CreateRouteCommand, ApiResponse<RouteDto>>
    {
        private readonly IRouteRepository _repository;
        private readonly IAuditLogRepository _auditLog;

        public CreateRouteCommandHandler(IRouteRepository repository, IAuditLogRepository auditLog)
        {
            _repository = repository;
            _auditLog = auditLog;
        }

        public async Task<ApiResponse<RouteDto>> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var route = new Route
                {
                    RouteCode = request.dto.RouteCode,
                    OriginAirportId = request.dto.OriginAirportId,
                    DestinationAirportId = request.dto.DestinationAirportId,
                    Distance = request.dto.Distance,
                    EstimatedDuration = request.dto.EstimatedDuration
                };

                var created = await _repository.CreateRouteAsync(route);

                await _auditLog.CreateAuditLogAsync(null, "ROUTE_CREATED", "Routes", created.RouteId);

                var dto = new RouteDto
                {
                    RouteId = created.RouteId,
                    RouteCode = created.RouteCode,
                    OriginAirportId = created.OriginAirportId,
                    DestinationAirportId = created.DestinationAirportId,
                    Distance = created.Distance,
                    EstimatedDuration = created.EstimatedDuration,
                    //OriginCode = created.OriginCode,
                    //OriginName = created.OriginName,
                    //OriginCity = created.OriginCity,
                    //OriginCountry = created.OriginCountry,
                    //DestinationCode = created.DestinationCode,
                    //DestinationName = created.DestinationName,
                    //DestinationCity = created.DestinationCity,
                    //DestinationCountry = created.DestinationCountry
                };

                return ApiResponse<RouteDto>.SuccessResponse(dto, "Route created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<RouteDto>.ErrorResponse("Failed to create route", new[] { ex.Message });
            }
        }
    }
}
