using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class FlightDetails
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string FlightStatus { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string DepartureGate { get; set; }
        public string ArrivalGate { get; set; }
        public decimal EconomyPrice { get; set; }
        public decimal BusinessPrice { get; set; }
        public decimal FirstClassPrice { get; set; }

        // Route info
        public int RouteId { get; set; }
        public string RouteCode { get; set; }
        public int Distance { get; set; }
        public int EstimatedDuration { get; set; }

        // Origin Airport
        public int OriginAirportId { get; set; }
        public string OriginCode { get; set; }
        public string OriginName { get; set; }
        public string OriginCity { get; set; }
        public string OriginCountry { get; set; }

        // Destination Airport
        public int DestinationAirportId { get; set; }
        public string DestinationCode { get; set; }
        public string DestinationName { get; set; }
        public string DestinationCity { get; set; }
        public string DestinationCountry { get; set; }

        // Aircraft
        public int? AircraftId { get; set; }
        public string AircraftModel { get; set; }
        public string Manufacturer { get; set; }
        public int? TotalSeats { get; set; }

        // Layovers
        public List<RouteLayover> Layovers { get; set; }
    }
}
