using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class MealPreference
    {
        public int MealPreferenceId { get; set; }
        public int BookingPassengerId { get; set; }
        public string MealType { get; set; }
        public string SpecialInstructions { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
