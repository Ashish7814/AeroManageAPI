using AeroManage.FlightManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Queries.Flights.Notification
{
    public record GetFlightNotificationsQuery(int flightId) : IRequest<ApiResponse<IEnumerable<FlightNotificationDto>>>;
}
