using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.Shared.DTos
{
    public class BookingFlightDto
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
    }

    public class BookingPassengerDto
    {
        public int BookingPassengerId { get; set; }
        public int BookingId { get; set; }
        public int PassengerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassengerType { get; set; }
        public string SeatClass { get; set; }
        public string SeatNumber { get; set; }
        public string TicketNumber { get; set; }
        public string MealPreference { get; set; }
        public string SpecialAssistance { get; set; }
        public int ExtraBaggage { get; set; }
        public int TravelInsurance { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? PassportExpiry { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FrequentFlyerNumber { get; set; }
    }
}
