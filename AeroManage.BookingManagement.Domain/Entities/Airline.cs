using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class Airline
    {
        public int AirlineId { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public string Country { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
