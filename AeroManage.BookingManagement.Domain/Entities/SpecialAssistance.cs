using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class SpecialAssistance
    {
        public int AssistanceId { get; set; }
        public int BookingPassengerId { get; set; }
        public string AssistanceType { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
