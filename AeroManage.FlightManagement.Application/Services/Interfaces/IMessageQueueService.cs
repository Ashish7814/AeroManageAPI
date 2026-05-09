using AeroManage.FlightManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Services.Interfaces
{
    public interface IMessageQueueService
    {
        Task PublishFlightNotificationAsync(FlightNotificationDto notification);
        Task PublishGateChangeAsync(int flightId, string newGate, string reason);
        Task PublishFlightDelayAsync(int flightId, int delayMinutes, string reason);
        Task PublishFlightStatusChangeAsync(int flightId, string oldStatus, string newStatus);
        void StartConsuming();
    }
}
