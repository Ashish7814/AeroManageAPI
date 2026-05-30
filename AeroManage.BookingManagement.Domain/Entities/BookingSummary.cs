using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class BookingSummary
    {
        public int BookingId { get; set; }
        public string BookingReference { get; set; }
        public string PNR { get; set; }
        public string BookingStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingEmail { get; set; }
        public string BookingPhone { get; set; }

        // Aggregated data
        public List<FlightSummary> Flights { get; set; }
        public List<PassengerSummary> Passengers { get; set; }
        public PricingSummary Pricing { get; set; }
    }
}
