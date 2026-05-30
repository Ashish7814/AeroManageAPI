using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class FlightSearchResult
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string OriginCode { get; set; }
        public string OriginCity { get; set; }
        public string DestinationCode { get; set; }
        public string DestinationCity { get; set; }
        public string AircraftModel { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public int Stops { get; set; }
        public int EstimatedDuration { get; set; }
        public int TotalRecords { get; set; }
    }
}
