using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class Airport
    {
        public int AirportId { get; set; }
        public string AirportCode { get; set; }
        public string ICAOCode { get; set; }
        public string AirportName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Timezone { get; set; }
        public string Regions { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalRecords { get; set; }
        public ICollection<Flight> DepartingFlights { get; set; }
        public ICollection<Flight> ArrivingFlights { get; set; }
    }
}
