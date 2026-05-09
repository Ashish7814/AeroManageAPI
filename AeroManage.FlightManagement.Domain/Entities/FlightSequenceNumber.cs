using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class FlightSequenceNumber
    {
        public int SequenceId { get; set; }
        public string Prefix { get; set; }
        public int CurrentNumber { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
