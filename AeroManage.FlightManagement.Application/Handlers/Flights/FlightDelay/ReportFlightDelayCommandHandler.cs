using AeroManage.FlightManagement.Application.Commands.Flights.FlightDelay;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Hubs;
using AeroManage.FlightManagement.Application.Services.Interfaces;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightDelay
{
    public class ReportFlightDelayCommandHandler : IRequestHandler<ReportFlightDelayCommand, ApiResponse<FlightDto>>
    {
        private readonly IFlightDelayRepository _repository;
        private readonly IHubContext<FlightHub> _hubContext;
        private readonly IMessageQueueService _messageQueue;

        public ReportFlightDelayCommandHandler(
            IFlightDelayRepository repository,
            IHubContext<FlightHub> hubContext,
            IMessageQueueService messageQueue)
        {
            _repository = repository;
            _hubContext = hubContext;
            _messageQueue = messageQueue;
        }

        public async Task<ApiResponse<FlightDto>> Handle(ReportFlightDelayCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var flight = await _repository.ReportDelayAsync(
                    request.dto.FlightId,
                    request.dto.DelayType,
                    request.dto.DelayMinutes,
                    request.dto.Reason,
                    request.dto.ReportedBy
                );

                var dto = new FlightDto
                {
                    FlightId = flight.FlightId,
                    FlightNumber = flight.FlightNumber,
                    FlightStatus = flight.FlightStatus,
                    DepartureDateTime = flight.DepartureDateTime
                };

                // Broadcast delay via SignalR
                await _hubContext.Clients.All.SendAsync("FlightDelayed", new
                {
                    request.dto.FlightId,
                    flight.FlightNumber,
                    request.dto.DelayMinutes,
                    request.dto.Reason,
                    request.dto.DelayType
                }, cancellationToken);

                // Publish to message queue
                await _messageQueue.PublishFlightDelayAsync(request.dto.FlightId, request.dto.DelayMinutes, request.dto.Reason);

                return ApiResponse<FlightDto>.SuccessResponse(dto, "Delay reported successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<FlightDto>.ErrorResponse("Failed to report delay", new[] { ex.Message });
            }
        }
    }

}
