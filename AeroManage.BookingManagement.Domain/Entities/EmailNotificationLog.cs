using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class EmailNotificationLog
    {
        public int NotificationId { get; set; }
        public int BookingId { get; set; }
        public string EmailType { get; set; }
        public string RecipientEmail { get; set; }
        public string Subject { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
