using AeroManage.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class Aircraft
    {
        public int AircraftId { get; set; }
        public string RegistrationNumber { get; set; }
        public string AircraftType { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public int YearManufactured { get; set; }
        public int EconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public int FirstClassSeats { get; set; }
        public int TotalSeats => EconomySeats + BusinessSeats + FirstClassSeats;
        public AircraftStatus CurrentStatus { get; set; } // Available, InFlight, Maintenance, OutOfService
        public DateTime LastMaintenanceDate { get; set; }
        public DateTime NextMaintenanceDate { get; set; }
        public bool IsActive { get; set; }
        public string CurrentLocation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int AircraftAge => DateTime.Now.Year - YearManufactured;
        public int TotalRecords { get; set; }
        public ICollection<Flight> Flights { get; set; }
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; } = new List<MaintenanceRecord>();
    }
}
