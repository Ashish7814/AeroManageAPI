using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class RecurrenceDay
    {
        public int Id { get; set; }
        public int RecurrenceId { get; set; }
        public string DayOfWeek { get; set; }

        public Recurrence Recurrence { get; set; }
    }
}
