using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class PassengerTicket
    {
        public int BookingPassengerId { get; set; }
        public string TicketNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SeatNumber { get; set; }
        public string SeatClass { get; set; }
        public string BoardingPass { get; set; }
        public string QRCode { get; set; }
    }
}
