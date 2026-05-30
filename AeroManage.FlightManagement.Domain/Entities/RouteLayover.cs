using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class RouteLayover
    {
        public int LayoverId { get; set; }
        public int RouteId { get; set; }
        public int AirportId { get; set; }
        public int LayoverSequence { get; set; }
        public int MinimumLayoverMinutes { get; set; }
        public int MaximumLayoverMinutes { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public string AirportCode { get; set; }
        public string AirportName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
