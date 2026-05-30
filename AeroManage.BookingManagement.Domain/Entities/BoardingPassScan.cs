using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class BoardingPassScan
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SeatNumber { get; set; }
        public string SeatClass { get; set; }
        public string FlightNumber { get; set; }
        public string BoardingGroup { get; set; }
        public string Gate { get; set; }
    }
}
