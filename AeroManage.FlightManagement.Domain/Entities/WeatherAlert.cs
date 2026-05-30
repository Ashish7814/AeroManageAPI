using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class WeatherAlert
    {
        public int AlertId { get; set; }
        public int AirportId { get; set; }
        public string AlertType { get; set; } // Storm, Fog, Snow, Wind, Ice
        public string Severity { get; set; } // Low, Medium, High, Severe
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public string AirportCode { get; set; }
        public string AirportName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
