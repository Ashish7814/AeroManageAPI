using AeroManage.FlightManagement.Application.Commands.Flights.Notification;
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

namespace AeroManage.FlightManagement.Application.Handlers.Flights.Notification
{
    public class CreateFlightNotificationCommandHandler : IRequestHandler<CreateFlightNotificationCommand, ApiResponse<FlightNotificationDto>>
    {
        private readonly IFlightNotificationRepository _repository;
        private readonly IHubContext<FlightHub> _hubContext;
        private readonly IMessageQueueService _messageQueue;

        public CreateFlightNotificationCommandHandler(
            IFlightNotificationRepository repository,
            IHubContext<FlightHub> hubContext,
            IMessageQueueService messageQueue)
        {
            _repository = repository;
            _hubContext = hubContext;
            _messageQueue = messageQueue;
        }

        public async Task<ApiResponse<FlightNotificationDto>> Handle(CreateFlightNotificationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var notification = await _repository.CreateNotificationAsync(
                    request.dto.FlightId,
                    request.dto.NotificationType,
                    request.dto.Message,
                    request.dto.Severity,
                    request.dto.CreatedBy
                );

                var dto = new FlightNotificationDto
                {
                    NotificationId = notification.NotificationId,
                    FlightId = notification.FlightId,
                    NotificationType = notification.NotificationType,
                    Message = notification.Message,
                    Severity = notification.Severity,
                    IsResolved = notification.IsResolved,
                    CreatedAt = notification.CreatedAt
                };

                // Send real-time notification via SignalR
                await _hubContext.Clients.Group($"Flight_{request.dto.FlightId}")
                    .SendAsync("FlightNotification", dto, cancellationToken);

                // Publish to message queue for external systems
                await _messageQueue.PublishFlightNotificationAsync(dto);

                return ApiResponse<FlightNotificationDto>.SuccessResponse(dto, "Notification created and sent");
            }
            catch (Exception ex)
            {
                return ApiResponse<FlightNotificationDto>.ErrorResponse("Failed to create notification", new[] { ex.Message });
            }
        }
    }
}
