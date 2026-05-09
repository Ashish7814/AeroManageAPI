using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class FlightCalendarView
    {
        public DateTime Date { get; set; }
        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }
        public string SeatClass { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int FlightCount { get; set; }
        public int TotalAvailableSeats { get; set; }
    }
}
