using AeroManage.UserManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class FlightDelayReason
    {
        public int DelayId { get; set; }
        public int FlightId { get; set; }
        public string DelayType { get; set; } // Weather, Technical, Operational, AirTrafficControl, CrewIssue
        public int DelayMinutes { get; set; }
        public string Reason { get; set; }
        public DateTime ReportedAt { get; set; }
        public int? ReportedBy { get; set; }

        // Navigation properties
        public User ReportedByName { get; set; }
    }
}
