using AeroManage.FlightManagement.Application.Commands.Flights.FlightNumber;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroMange.Shared.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightNumber
{
    public class GenerateFlightNumberCommandHandler : IRequestHandler<GenerateFlightNumberCommand, ApiResponse<AeroManage.Shared.DTos.FlightNumberResultDto>>
    {
        private readonly IFlightNumberRepository _repository;
        private readonly IAuditLogRepository _auditLog;

        public GenerateFlightNumberCommandHandler(
            IFlightNumberRepository repository,
            IAuditLogRepository auditLog)
        {
            _repository = repository;
            _auditLog = auditLog;
        }

        public async Task<ApiResponse<AeroManage.Shared.DTos.FlightNumberResultDto>> Handle(GenerateFlightNumberCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repository.GenerateFlightNumberAsync(request.dto.Prefix);

                if (result == null)
                {
                    return ApiResponse<AeroManage.Shared.DTos.FlightNumberResultDto>.ErrorResponse("Failed to generate flight number");
                }

                await _auditLog.CreateAuditLogAsync(
                    null,
                    "FLIGHT_NUMBER_GENERATED",
                    "FlightSequenceNumbers",
                    result.SequenceNumber,
                    null,
                    $"Generated flight number: {result.FlightNumber}"
                );

                return ApiResponse<AeroManage.Shared.DTos.FlightNumberResultDto>.SuccessResponse(result, $"Flight number generated: {result.FlightNumber}");
            }
            catch (Exception ex)
            {
                return ApiResponse<AeroManage.Shared.DTos.FlightNumberResultDto>.ErrorResponse("Failed to generate flight number", new[] { ex.Message });
            }
        }
    }
}
