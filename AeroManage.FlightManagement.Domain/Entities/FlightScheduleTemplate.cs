using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class FlightScheduleTemplate
    {
        //public int TemplateId { get; set; }
        //public string TemplateName { get; set; }
        //public string FlightNumberPrefix { get; set; }
        //public int RouteId { get; set; }
        //public int? AircraftId { get; set; }
        //public string RecurrenceType { get; set; } // Daily, Weekly, Monthly
        //public string DaysOfWeek { get; set; } // Mon,Wed,Fri
        //public int? DayOfMonth { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime? EndDate { get; set; }
        //public TimeSpan DepartureTime { get; set; }
        //public TimeSpan ArrivalTime { get; set; }
        //public decimal? EconomyPrice { get; set; }
        //public decimal? BusinessPrice { get; set; }
        //public decimal? FirstClassPrice { get; set; }
        //public bool IsActive { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public int? CreatedBy { get; set; }

        //// Navigation properties
        //public string RouteCode { get; set; }
        //public string OriginCode { get; set; }
        //public string OriginCity { get; set; }
        //public string DestinationCode { get; set; }
        //public string DestinationCity { get; set; }
        //public string RegistrationNumber { get; set; }
        //public string AircraftType { get; set; }
        //public int TotalRecords { get; set; }

        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string FlightNumberPrefix { get; set; }

        public int RouteId { get; set; }
        public int? AircraftId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int TotalRecords { get; set; }

        // 🔗 Navigation
        public Route Route { get; set; }
        public Aircraft Aircraft { get; set; }
        public Recurrence Recurrence { get; set; }
        public ICollection<FlightPrice> Pricings { get; set; }
    }
}
