using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string BookingReference { get; set; }
        public string PNR { get; set; }
        public int? UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public string BookingEmail { get; set; }
        public string BookingPhone { get; set; }
        public string SpecialRequests { get; set; }
        public string BookingStatus { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentIntentId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public BookingFlight flight { get; set; }
        public ICollection<BookingPassenger> BookingPassengers { get; set; }
    }
}
