using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class BoardingPass
    {
        public int BoardingPassId { get; set; }
        public int BookingPassengerId { get; set; }
        public string BoardingPassNumber { get; set; }
        public string Gate { get; set; }
        public DateTime BoardingTime { get; set; }
        public string BoardingGroup { get; set; }
        public int BoardingZone { get; set; }
        public string QRCode { get; set; }
        public string Barcode { get; set; }
        public bool IsUsed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string SeatNumber { get; set; }
        public string SeatClass { get; set; }
        public DateTime? ScannedAt { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
