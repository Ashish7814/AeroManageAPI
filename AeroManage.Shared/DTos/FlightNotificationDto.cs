using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.Shared.DTos
{
    public class FlightNotificationDto
    {
        public int NotificationId { get; set; }
        public int FlightId { get; set; }
        public string NotificationType { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDto ChangedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
    public class UserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public RoleDto Role { get; set; }
    }
    public class FlightStatusHistoryDto
    {
        public int StatusHistoryId { get; set; }
        public int FlightId { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public string Reason { get; set; }
        public UserDto ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
    }
    public class RoleDto
    {
        public string RoleName { get; set; }
    }
    public class FlightDelayReasonDto
    {
        public int DelayId { get; set; }
        public int FlightId { get; set; }
        public string DelayType { get; set; }
        public int DelayMinutes { get; set; }
        public string Reason { get; set; }
        public DateTime ReportedAt { get; set; }
        public UserDto ReportedBy { get; set; }
    }
}
