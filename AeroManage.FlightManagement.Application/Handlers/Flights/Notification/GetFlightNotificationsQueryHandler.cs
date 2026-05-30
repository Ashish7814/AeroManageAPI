using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Flights.Notification;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights.Notification
{
    public class GetFlightNotificationsQueryHandler : IRequestHandler<GetFlightNotificationsQuery, ApiResponse<IEnumerable<FlightNotificationDto>>>
    {
        private readonly IFlightNotificationRepository _repository;

        public GetFlightNotificationsQueryHandler(IFlightNotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<FlightNotificationDto>>> Handle(GetFlightNotificationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //var notifications = await _repository.GetFlightNotificationsAsync(request.dto.FlightId, request.dto.IncludeResolved);

                //var dtos = notifications.Select(n => new FlightNotificationDto
                //{
                //    NotificationId = n.NotificationId,
                //    FlightId = n.FlightId,
                //    NotificationType = n.NotificationType,
                //    Message = n.Message,
                //    Severity = n.Severity,
                //    IsResolved = n.IsResolved,
                //    CreatedAt = n.CreatedAt,
                //    CreatedByName = n.CreatedByName,
                //    ResolvedAt = n.ResolvedAt
                //});


                var dtos = new List<FlightNotificationDto>
                {
                    new FlightNotificationDto
                    {
                        NotificationId = 1,
                        FlightId = request.flightId,
                        NotificationType = "Delay",
                        Message = "Flight delayed 85 minutes due to thunderstorm.",
                        Severity = "Critical",
                        IsResolved = false,
                        CreatedAt = DateTime.Parse("2026-03-05T10:08:00"),
                        ChangedBy = new UserDto
                        {
                            UserId = 1,
                            FirstName = "System",
                            LastName = "Auto",
                            Email = "system@airline.com"
                        },
                        ResolvedAt = DateTime.Parse("2026-03-05T10:08:00")
                    },
                    new FlightNotificationDto
                    {
                        NotificationId = 2,
                        FlightId = request.flightId,
                        NotificationType = "GateChange",
                        Message = "Gate changed from E5 to E7.",
                        Severity = "Warning",
                        IsResolved = false,
                        CreatedAt = DateTime.Parse("2026-03-05T11:45:00"),
                        ChangedBy = new UserDto
                        {
                            UserId = 2,
                            FirstName = "Ground",
                            LastName = "Control",
                            Email = "ground.control@airline.com"
                        },
                        ResolvedAt = DateTime.Parse("2026-03-05T10:08:00")
                    },
                   new FlightNotificationDto
                    {
                        NotificationId = 3,
                        FlightId = request.flightId,
                        NotificationType = "Boarding",
                        Message = "Boarding started for passengers.",
                        Severity = "Info",
                        IsResolved = true,
                        CreatedAt = DateTime.Parse("2026-03-05T10:08:00"),
                        ChangedBy = new UserDto
                        {
                            UserId = 3,
                            FirstName = "Arjun",
                            LastName = "Kumar",
                            Email = "arjun.kumar@airline.com"
                        },
                        ResolvedAt = DateTime.Parse("2026-03-05T10:08:00")
                    },


                };

                return ApiResponse<IEnumerable<FlightNotificationDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<FlightNotificationDto>>.ErrorResponse("Error fetching notifications", new[] { ex.Message });
            }
        }
    }
}


