using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class MaintenanceRecord
    {
        public int MaintenanceRecordId { get; set; }
        public DateTime DatePerformed { get; set; }
        public string PerformedBy { get; set; }
        public string Description { get; set; }
        public DateTime NextDueDate { get; set; }
    }
}
