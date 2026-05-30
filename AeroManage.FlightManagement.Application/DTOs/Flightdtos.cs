using AeroManage.FlightManagement.Domain.Entities;
using AeroManage.Shared.Enums;
using AeroManage.UserManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.DTOs
{
    public class CreateAirportDto
    {
        [Required]
        [StringLength(10)]
        public string AirportCode { get; set; }

        [StringLength(10)]
        public string ICAOCode { get; set; }

        [Required]
        [StringLength(200)]
        public string AirportName { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string State { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        [StringLength(50)]
        public string Timezone { get; set; }
    }

    public class AirportDto
    {
        public int AirportId { get; set; }
        public string AirportCode { get; set; }
        public string ICAOCode { get; set; }
        public string AirportName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Timezone { get; set; }
        public string Regions { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<FlightDto> DepartingFlights { get; set; }
        public List<FlightDto> ArrivingFlights { get; set; }
        public int TotalRecords { get; set; }
    }

    public class FlightPriceDto
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public string ClassType { get; set; } // Economy, Business, First

        public decimal Price { get; set; }

        public FlightDto Flight { get; set; }
    }

    // ==================== AIRCRAFT DTOs ====================
    public class CreateAircraftDto
    {
        [Required]
        [StringLength(50)]
        public string RegistrationNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string AircraftType { get; set; }

        [StringLength(100)]
        public string Manufacturer { get; set; }

        [StringLength(100)]
        public string Model { get; set; }

        public int YearManufactured { get; set; }

        [Required]
        [Range(1, 1000)]
        public int TotalSeats { get; set; }

        [Required]
        [Range(0, 1000)]
        public int EconomySeats { get; set; }

        [Required]
        [Range(0, 200)]
        public int BusinessSeats { get; set; }

        [Required]
        [Range(0, 100)]
        public int FirstClassSeats { get; set; }
    }

    public class AircraftDto
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
        public int TotalSeats { get; set; }
        public string CurrentStatus { get; set; } // Available, InFlight, Maintenance, OutOfService
        public DateTime LastMaintenanceDate { get; set; }
        public DateTime NextMaintenanceDate { get; set; }
        public bool IsActive { get; set; }
        public string CurrentLocation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int AircraftAge { get; set; }
        public int TotalRecords { get; set; }
        public List<FlightDto> Flights { get; set; }
        public List<MaintenanceRecordDto> MaintenanceRecordDtos { get; set; }
    }

    public class UpdateAircraftStatusDto
    {
        public int AircraftId { get; set; }
        [Required]
        public string Status { get; set; } // Available, InFlight, Maintenance, OutOfService
    }

    public class AircraftSearchDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        //public string Status { get; set; }
    }

    public class AirportSearchDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SearchTerm { get; set; }
        //public string Country { get; set; } = null;
    }

    public class RouteSearchDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    // ==================== ROUTE DTOs ====================
    public class CreateRouteDto
    {
        [Required]
        [StringLength(20)]
        public string RouteCode { get; set; }

        [Required]
        public int OriginAirportId { get; set; }

        [Required]
        public int DestinationAirportId { get; set; }

        public int Distance { get; set; }
        public int EstimatedDuration { get; set; }
    }

    public class RouteDto
    {
        public int RouteId { get; set; }
        public string RouteCode { get; set; }

        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }

        public int Distance { get; set; }
        public int EstimatedDuration { get; set; }

        public AirportDto OriginAirport { get; set; }
        public AirportDto DestinationAirport { get; set; }

        public List<FlightDto> Flights { get; set; }
        public int TotalRecords { get; set; }
    }

    // ==================== FLIGHT DTOs ====================
    public class CreateFlightDto
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
        public RouteDto Route { get; set; }
        public AircraftDto Aircraft { get; set; }

        public List<FlightPriceDto> Prices { get; set; }
        public List<FlightSeatAvailabilityDto> SeatAvailability { get; set; }
        public List<GateAssignmentDto> GateAssignments { get; set; }
        //[Required]
        //[StringLength(20)]
        //public string FlightNumber { get; set; }

        //[Required]
        //public int RouteId { get; set; }

        //[Required]
        //public int AircraftId { get; set; }

        //[Required]
        //public DateTime DepartureDateTime { get; set; }

        //[Required]
        //public DateTime ArrivalDateTime { get; set; }

        //[Required]
        //[Range(0, 10000)]
        //public decimal EconomyPrice { get; set; }

        //[Required]
        //[Range(0, 50000)]
        //public decimal BusinessPrice { get; set; }

        //[Required]
        //[Range(0, 100000)]
        //public decimal FirstClassPrice { get; set; }

        //[StringLength(10)]
        //public string DepartureGate { get; set; }

        //[StringLength(10)]
        //public string ArrivalGate { get; set; }
        //public int CreatedBy { get; set; }
    }

    public class FlightDto
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

        public string DepartureGate { get; set; }
        public string ArrivalGate { get; set; }

        public decimal EconomyPrice { get; set; }
        public decimal BusinessPrice { get; set; }
        public decimal FirstClassPrice { get; set; }

        public int AvailableEconomySeats { get; set; }
        public int AvailableBusinessSeats { get; set; }
        public int AvailableFirstClassSeats { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalRecords { get; set; }

        // Navigation
        public RouteDto Route { get; set; }
        public AircraftDto Aircraft { get; set; }
    }
    public class FlightSeatAvailabilityDto
    {
        public int Id { get; set; }

        public int FlightId { get; set; }
        public string ClassType { get; set; }

        public int AvailableSeats { get; set; }

        public FlightDto Flight { get; set; }


    }

    public class FlightSearchDto
    {
        public int? OriginAirportId { get; set; }
        public int? DestinationAirportId { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string? FlightNumber { get; set; }
        public int? PageNumber { get; set; } = 1;
        public int? PageSize { get; set; } = 20;
        public string? SearchTerm { get; set; }
        public string? Country { get; set; }
    }

    public class UpdateFlightStatusDto
    {
        [Required]
        //public string Status { get; set; } // Scheduled, Boarding, Departed, InFlight, Landed, Delayed, Cancelled
        //public string Reason { get; set; }
        public int FlightId { get; set; }

        public string FlightStatus { get; set; }
        public int ChangedBy { get; set; }
    }

    public class AssignFlightCrewDto
    {
        public int FlightId { get; set; }
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string CrewRole { get; set; } // Pilot, CoPilot, FlightAttendant
    }

    //public class FlightCrewDto
    //{
    //    public int FlightCrewId { get; set; }
    //    public int FlightId { get; set; }
    //    public int UserId { get; set; }
    //    public string CrewRole { get; set; }
    //    public DateTime AssignedAt { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Email { get; set; }
    //    public string PhoneNumber { get; set; }
    //}

    public class FlightCrewDto
    {
        public int FlightCrewId { get; set; }
        public int FlightId { get; set; }
        public int UserId { get; set; }
        public DateTime AssignedAt { get; set; }

        public CrewUserDto Users { get; set; }
    }

    public class CrewUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public RoleDto Roles { get; set; }
    }

    public class RoleDto
    {
        public string RoleName { get; set; }
    }

    public class FlightStatusHistoryDto
    {
        public int StatusHistoryId { get; set; }
        public int FlightId { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public string Reason { get; set; }
        public UserDto ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
    }
    public class UserDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public RoleDto Role { get; set; }
    }

    public class MaintenanceRecordDto
    {
        public int MaintenanceRecordId { get; set; }
        public DateTime DatePerformed { get; set; }
        public string PerformedBy { get; set; }
        public string Description { get; set; }
        public DateTime NextDueDate { get; set; }
    }



    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string[] Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, string[] errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }

    public class PagedResultDto<T>
    {
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();

        public int TotalRecords { get; set; }

        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        //public int? TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        /*public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalRecords / PageSize);*/
    }
}

