using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class BookingModificationLog
    {
        public int ModificationId { get; set; }
        public int BookingId { get; set; }
        public string ModificationType { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public decimal ModificationFee { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string Reason { get; set; }
    }
}
