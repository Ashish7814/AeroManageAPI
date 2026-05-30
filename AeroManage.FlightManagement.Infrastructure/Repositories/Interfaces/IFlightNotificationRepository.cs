using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces
{
    public interface IFlightNotificationRepository
    {
        Task<FlightNotification> CreateNotificationAsync(int flightId, string notificationType, string message, string severity, int createdBy);
        Task<IEnumerable<FlightNotification>> GetFlightNotificationsAsync(int flightId, bool includeResolved);
        Task<FlightNotification> ResolveNotificationAsync(int notificationId, int resolvedBy);
    }
}
