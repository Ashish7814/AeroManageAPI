using AeroManage.UserManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class FlightNotification
    {
        public int NotificationId { get; set; }
        public int FlightId { get; set; }
        public string NotificationType { get; set; } // Delay, GateChange, Cancellation, Weather, Boarding
        public string Message { get; set; }
        public string Severity { get; set; } // Info, Warning, Critical
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
        public User ChangedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }

    }
}
