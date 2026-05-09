using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public string PaymentReference { get; set; }
        public string PaymentIntentId { get; set; }
        public string PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string CardBrand { get; set; }
        public string CardLast4 { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string TransactionId { get; set; }
        public decimal? RefundAmount { get; set; }
        public DateTime? RefundDate { get; set; }
        public string Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
