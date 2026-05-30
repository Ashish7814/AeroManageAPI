using AeroManage.FlightManagement.Application.Commands.Flights.FlightGate;
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

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightGate
{
    public class ChangeFlightGateCommandHandler : IRequestHandler<ChangeFlightGateCommand, ApiResponse<FlightDto>>
    {
        private readonly IGateAssignmentRepository _repository;
        private readonly IHubContext<FlightHub> _hubContext;
        private readonly IMessageQueueService _messageQueue;

        public ChangeFlightGateCommandHandler(
            IGateAssignmentRepository repository,
            IHubContext<FlightHub> hubContext,
            IMessageQueueService messageQueue)
        {
            _repository = repository;
            _hubContext = hubContext;
            _messageQueue = messageQueue;
        }

        public async Task<ApiResponse<FlightDto>> Handle(ChangeFlightGateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var flight = await _repository.ChangeFlightGateAsync(
                    request.dto.FlightId,
                    request.dto.NewGateNumber,
                    request.dto.GateType,
                    request.dto.Reason,
                    request.dto.ChangedBy
                );

                var dto = new FlightDto
                {
                    FlightId = flight.FlightId,
                    FlightNumber = flight.FlightNumber,
                    //DepartureGate = flight.DepartureGate,
                    //ArrivalGate = flight.ArrivalGate,
                    FlightStatus = flight.FlightStatus
                };

                // Broadcast gate change via SignalR
                await _hubContext.Clients.All.SendAsync("GateChanged", new
                {
                    request.dto.FlightId,
                    flight.FlightNumber,
                    OldGate = "Updated",
                    NewGate = request.dto.NewGateNumber,
                    request.dto.GateType,
                    request.dto.Reason
                }, cancellationToken);

                // Publish to RabbitMQ
                await _messageQueue.PublishGateChangeAsync(request.dto.FlightId, request.dto.NewGateNumber, request.dto.Reason);

                return ApiResponse<FlightDto>.SuccessResponse(dto, "Gate changed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<FlightDto>.ErrorResponse("Failed to change gate", new[] { ex.Message });
            }
        }
    }

}
