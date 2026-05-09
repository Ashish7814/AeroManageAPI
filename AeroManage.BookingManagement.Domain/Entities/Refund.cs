using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class Refund
    {
        public int RefundId { get; set; }
        public int BookingId { get; set; }
        public int PaymentId { get; set; }
        public string RefundReference { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal CancellationFee { get; set; }
        public string RefundStatus { get; set; }
        public string RefundReason { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public int RequestedBy { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string StripeRefundId { get; set; }
    }
}
