using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class PNRDetails
    {
        public string PNR { get; set; }
        public string BookingReference { get; set; }
        public Booking Booking { get; set; }
        public List<PassengerTicket> Passengers { get; set; }
        public List<FlightDetails> Flights { get; set; }
    }
}
