using AeroManage.UserManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Domain.Entities
{
    public class Flight
    {

        public int FlightId { get; set; }
        public string FlightNumber { get; set; }

        public int RouteId { get; set; }
        public int AircraftId { get; set; }

        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }

        public DateTime? ActualDepartureDateTime { get; set; }
        public DateTime? ActualArrivalDateTime { get; set; }

        public string FlightStatus { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public Route Route { get; set; }
        public Aircraft Aircraft { get; set; }

        public ICollection<FlightPrice> Prices { get; set; }
        public ICollection<FlightSeatAvailability> SeatAvailability { get; set; }
        public ICollection<GateAssignment> GateAssignments { get; set; }
        //public int FlightId { get; set; }
        //public string FlightNumber { get; set; }

        //public int RouteId { get; set; }
        //public int AircraftId { get; set; }

        //public DateTime DepartureDateTime { get; set; }
        //public DateTime ArrivalDateTime { get; set; }

        //public DateTime? ActualDepartureDateTime { get; set; }
        //public DateTime? ActualArrivalDateTime { get; set; }

        //public string FlightStatus { get; set; }

        //public string DepartureGate { get; set; }
        //public string ArrivalGate { get; set; }

        //public decimal EconomyPrice { get; set; }
        //public decimal BusinessPrice { get; set; }
        //public decimal FirstClassPrice { get; set; }

        //public int AvailableEconomySeats { get; set; }
        //public int AvailableBusinessSeats { get; set; }
        //public int AvailableFirstClassSeats { get; set; }

        //public int CreatedBy { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public DateTime UpdatedAt { get; set; }
        //public int TotalRecords { get; set; }

        //// Navigation
        //public Route Route { get; set; }
        //public Aircraft Aircraft { get; set; }
    }

    public class FlightCrew
    {
        public int FlightCrewId { get; set; }
        public int FlightId { get; set; }
        public int UserId { get; set; }

        public User users { get; set; }
        //public string CrewRole { get; set; } // Pilot, CoPilot, FlightAttendant
        public DateTime AssignedAt { get; set; }

        // Navigation properties
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public string Email { get; set; }
        //public string PhoneNumber { get; set; }
    }

    public class FlightStatusHistory
    {
        public int StatusHistoryId { get; set; }
        public int FlightId { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public string Reason { get; set; }
        public User ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangedByName { get; set; }
    }
}
