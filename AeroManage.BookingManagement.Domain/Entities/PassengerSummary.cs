using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class PassengerSummary
    {
        public int BookingPassengerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassengerType { get; set; }
        public string SeatClass { get; set; }
        public string SeatNumber { get; set; }
        public string TicketNumber { get; set; }
        public string MealPreference { get; set; }
        public string SpecialAssistance { get; set; }
        public string? FrequentFlyerNumber { get; set; }
        public bool ExtraBaggage { get; set; }
    }
}
