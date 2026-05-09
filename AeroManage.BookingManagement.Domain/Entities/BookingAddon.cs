using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class BookingAddon
    {
        public int AddonId { get; set; }
        public int BookingPassengerId { get; set; }
        public string AddonType { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
