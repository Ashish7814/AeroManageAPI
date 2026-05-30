using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class CalendarFare
    {
        public int RouteId { get; set; }
        public DateTime FlightDate { get; set; }
        public string SeatClass { get; set; }
        public decimal MinPrice { get; set; }
        public int AvailableFlights { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
