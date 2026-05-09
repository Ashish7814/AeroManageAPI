using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class FlightSeatAvailability
    {
        public int Id { get; set; }

        public int FlightId { get; set; }

        public string ClassType { get; set; }

        public int AvailableSeats { get; set; }

        public Flight Flight { get; set; }
    }
}
