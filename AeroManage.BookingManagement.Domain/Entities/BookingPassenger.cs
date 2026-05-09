using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class BookingPassenger
    {
        //public int BookingPassengerId { get; set; }
        //public int BookingId { get; set; }
        //public int PassengerId { get; set; }
        //public string PassengerType { get; set; }
        //public string SeatClass { get; set; }
        //public string SeatNumber { get; set; }
        //public string MealPreference { get; set; }
        //public string SpecialAssistance { get; set; }
        //public int ExtraBaggage { get; set; }
        //public string TicketNumber { get; set; }
        //public string ETicketPath { get; set; }
        //public string QRCodePath { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public bool TravelInsurance { get; set; }

        //// Navigation properties
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public string Email { get; set; }
        //public string Phone { get; set; }
        //public string FlightNumber { get; set; }
        //public DateTime DepartureDateTime { get; set; }
        //public DateTime ArrivalDateTime { get; set; }
        //public string OriginCode { get; set; }
        //public string DestinationCode { get; set; }


        public int BookingPassengerId { get; set; }
        public int BookingId { get; set; }
        public int PassengerId { get; set; }

        public string PassengerType { get; set; }
        public string SeatClass { get; set; }
        public string? SeatNumber { get; set; }

        public string? MealPreference { get; set; }
        public string? SpecialAssistance { get; set; }

        public int ExtraBaggage { get; set; }

        public string? TicketNumber { get; set; }
        public string? ETicketPath { get; set; }
        public string? QRCodePath { get; set; }

        public bool TravelInsurance { get; set; }

        public DateTime CreatedAt { get; set; }

        // ✅ Proper navigation
        public Passenger Passenger { get; set; }
        public Booking Booking { get; set; }
    }
}
