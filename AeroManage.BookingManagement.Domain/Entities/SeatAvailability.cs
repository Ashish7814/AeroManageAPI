using AeroManage.BookingManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class SeatAvailability
    {
        public SeatClass SeatClass { get; set; }
        public int TotalSeats { get; set; }
        public int FirstClassAvailable { get; set; }
        public int BusinessAvailable { get; set; }
        public int EconomyAvailable { get; set; }
        public int AvailableSeats { get; set; }
    }
}
