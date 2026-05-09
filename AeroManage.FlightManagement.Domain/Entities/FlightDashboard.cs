using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class FlightDashboard
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string FlightStatus { get; set; }
        public string BoardingStatus { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public DateTime? ActualDepartureDateTime { get; set; }
        public DateTime? ActualArrivalDateTime { get; set; }
        public int DelayMinutes { get; set; }
        public string DepartureGate { get; set; }
        public string ArrivalGate { get; set; }
        public string RouteCode { get; set; }
        public string OriginCode { get; set; }
        public string OriginName { get; set; }
        public string OriginCity { get; set; }
        public string DestinationCode { get; set; }
        public string DestinationName { get; set; }
        public string DestinationCity { get; set; }
        public string RegistrationNumber { get; set; }
        public string AircraftType { get; set; }
        public int ActiveNotifications { get; set; }
        public string WeatherAlert { get; set; }
    }
}
