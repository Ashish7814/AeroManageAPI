using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class SeatReservation
    {
        public int ReservationId { get; set; }
        public int FlightId { get; set; }
        public int SeatId { get; set; }
        public int BookingPassengerId { get; set; }
        public string ReservationStatus { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
    }
}
