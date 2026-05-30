using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class Route
    {

        public int RouteId { get; set; }
        public string RouteCode { get; set; }

        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }

        public int Distance { get; set; }
        public int EstimatedDuration { get; set; }

        public Airport OriginAirport { get; set; }
        public Airport DestinationAirport { get; set; }

        public ICollection<Flight> Flights { get; set; }
        public int TotalRecords { get; set; }
    }
}
