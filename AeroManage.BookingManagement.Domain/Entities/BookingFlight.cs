using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class BookingFlight
    {
        public int BookingFlightId { get; set; }
        public int BookingId { get; set; }
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public int FlightSegment { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string OriginCode { get; set; }
        public string DestinationCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
