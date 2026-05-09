using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class PaymentWebhookLog
    {
        public int WebhookId { get; set; }
        public string EventType { get; set; }
        public string PayloadData { get; set; }
        public string PaymentIntentId { get; set; }
        public int? BookingId { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
