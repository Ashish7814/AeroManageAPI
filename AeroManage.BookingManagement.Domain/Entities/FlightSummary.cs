using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class FlightSummary
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string AirlineCode { get; set; } = string.Empty;
        public string AirlineName { get; set; } = string.Empty;
        public string AircraftType { get; set; } = string.Empty;
        public string CabinClass { get; set; } = string.Empty;
        public DateTime DepartureDateTime { get; set; }
        public AirportInfo Departure { get; set; } = new();
        public AirportInfo Arrival { get; set; } = new();
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string BaggageAllowance { get; set; } = string.Empty;
        public DateTime ArrivalDateTime { get; set; }
    }
    public class AirportInfo
    {
        public string AirportCode { get; set; } = string.Empty;
        public string AirportName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public DateTime DateTime { get; set; }
        public string Terminal { get; set; } = string.Empty;
        public string Gate { get; set; } = string.Empty;
    }
}
