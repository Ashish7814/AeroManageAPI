using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.DTOs
{
    // ==================== ROUTE LAYOVER DTOs ====================
    public class AddRouteLayoverDto
    {
        [Required]
        public int AirportId { get; set; }
        public int RouteId { get; set; }

        [Required]
        [Range(1, 10)]
        public int LayoverSequence { get; set; }

        [Range(30, 1440)]
        public int MinimumLayoverMinutes { get; set; } = 60;

        [Range(30, 1440)]
        public int MaximumLayoverMinutes { get; set; } = 240;
    }

    public class RouteLayoverDto
    {
        public int LayoverId { get; set; }
        public int RouteId { get; set; }
        public int AirportId { get; set; }
        public int LayoverSequence { get; set; }
        public int MinimumLayoverMinutes { get; set; }
        public int MaximumLayoverMinutes { get; set; }
        public string AirportCode { get; set; }
        public string AirportName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    public class RouteWithLayoversDto
    {
        public RouteDto Route { get; set; }
        public RouteLayoverDto[] Layovers { get; set; }
    }

    // ==================== FLIGHT SCHEDULE TEMPLATE DTOs ====================
    public class CreateFlightScheduleTemplateDto
    {
        [Required]
        [StringLength(100)]
        public string TemplateName { get; set; }

        [Required]
        [StringLength(10)]
        public string FlightNumberPrefix { get; set; }

        [Required]
        public int RouteId { get; set; }

        public int? AircraftId { get; set; }

        [Required]
        [RegularExpression("Daily|Weekly|Monthly")]
        public string RecurrenceType { get; set; }

        public string DaysOfWeek { get; set; } // e.g., "Mon,Wed,Fri"

        [Range(1, 31)]
        public int? DayOfMonth { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public TimeSpan DepartureTime { get; set; }

        [Required]
        public TimeSpan ArrivalTime { get; set; }

        [Range(0, 10000)]
        public decimal? EconomyPrice { get; set; }

        [Range(0, 50000)]
        public decimal? BusinessPrice { get; set; }

        [Range(0, 100000)]
        public decimal? FirstClassPrice { get; set; }
        public int CreatedBy { get; set; }
    }

    public class FlightScheduleTemplateDto
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string FlightNumberPrefix { get; set; }

        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }

        // 🔹 Route
        public RouteDto Route { get; set; }

        // 🔹 Aircraft
        public AircraftDto Aircraft { get; set; }

        // 🔹 Recurrence
        public RecurrenceDto Recurrence { get; set; }

        // 🔹 Pricing
        public List<FlightPriceDto> Pricings { get; set; }
    }

    public class RecurrenceDto
    {
        public string RecurrenceType { get; set; }
        public List<string> DaysOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
    }


    public class GenerateFlightsDto
    {
        [Required]
        public DateTime GenerateFromDate { get; set; }

        [Required]
        public DateTime GenerateToDate { get; set; }
        public int TemplateId { get; set; }
        public int CreatedBy { get; set; }
    }

    public class GenerateFlightsResultDto
    {
        public int FlightsCreated { get; set; }
        public string Message { get; set; }
    }

    // ==================== FLIGHT NOTIFICATION DTOs ====================
    public class CreateFlightNotificationDto
    {
        [Required]
        public int FlightId { get; set; }
        [Required]
        public string NotificationType { get; set; } // Delay, GateChange, Cancellation, Weather, Boarding

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }

        [RegularExpression("Info|Warning|Critical")]
        public string Severity { get; set; } = "Info";
        public int CreatedBy { get; set; }
    }

    public class FlightNotificationDto
    {
        public int NotificationId { get; set; }
        public int FlightId { get; set; }
        public string NotificationType { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDto ChangedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
   

    // ==================== WEATHER ALERT DTOs ====================
    public class CreateWeatherAlertDto
    {
        [Required]
        public int AirportId { get; set; }

        [Required]
        public string AlertType { get; set; } // Storm, Fog, Snow, Wind, Ice

        [Required]
        [RegularExpression("Low|Medium|High|Severe")]
        public string Severity { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }

    public class WeatherAlertDto
    {
        public int AlertId { get; set; }
        public int AirportId { get; set; }
        public string AlertType { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
        public string AirportCode { get; set; }
        public string AirportName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    // ==================== GATE ASSIGNMENT DTOs ====================
    public class AssignGateDto
    {
        public int FlightId { get; set; }
        [Required]
        [StringLength(10)]
        public string GateNumber { get; set; }

        [Required]
        [RegularExpression("Departure|Arrival")]
        public string GateType { get; set; }

        [Required]
        public DateTime ScheduledTime { get; set; }
        public int AssignedBy { get; set; }
    }

    public class ChangeGateDto
    {
        [Required]
        public int FlightId { get;set; }
        [Required]
        [StringLength(10)]
        public string NewGateNumber { get; set; }

        [Required]
        [RegularExpression("Departure|Arrival")]
        public string GateType { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }
        public int ChangedBy { get; set; }
    }

    public class GateAssignmentDto
    {
        public int AssignmentId { get; set; }
        public int FlightId { get; set; }
        public string GateNumber { get; set; }
        public string GateType { get; set; }
        public DateTime ScheduledTime { get; set; }
        public DateTime? ActualTime { get; set; }
        public string Status { get; set; }
        public DateTime AssignedAt { get; set; }
    }

    // ==================== FLIGHT DELAY DTOs ====================
    public class ReportFlightDelayDto
    {
        public int FlightId { get; set; }
        [Required]
        public string DelayType { get; set; } // Weather, Technical, Operational, AirTrafficControl, CrewIssue

        [Required]
        [Range(1, 1440)]
        public int DelayMinutes { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; }
        public int ReportedBy { get; set; }
    }

    public class FlightDelayReasonDto
    {
        public int DelayId { get; set; }
        public int FlightId { get; set; }
        public string DelayType { get; set; }
        public int DelayMinutes { get; set; }
        public string Reason { get; set; }
        public DateTime ReportedAt { get; set; }
        public UserDto ReportedBy { get; set; }
    }

    // ==================== BOARDING STATUS DTOs ====================
    public class UpdateBoardingStatusDto
    {
        public int FlightId { get; set; }
        [Required]
        [RegularExpression("NotStarted|Boarding|Completed|Closed")]
        public string BoardingStatus { get; set; }
        public int UpdatedBy { get; set; }
    }

    // ==================== FLIGHT NUMBER GENERATION DTOs ====================
    public class GenerateFlightNumberDto
    {
        [Required]
        [StringLength(10)]
        public string Prefix { get; set; }
    }

    public class FlightNumberResultDto
    {
        public string FlightNumber { get; set; }
        public int SequenceNumber { get; set; }
    }

    // ==================== FLIGHT DASHBOARD DTOs ====================
    public class FlightDashboardQueryDto
    {
        public int? AirportId { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; }
    }

    public class FlightDashboardDto
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string FlightStatus { get; set; }
        public string BoardingStatus { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public DateTime? ActualDepartureDateTime { get; set; }
        public DateTime? ActualArrivalDateTime { get; set; }
        public int DelayMinutes { get; set; }
        public string DepartureGate { get; set; }
        public string ArrivalGate { get; set; }
        public string RouteCode { get; set; }
        public string OriginCode { get; set; }
        public string OriginName { get; set; }
        public string OriginCity { get; set; }
        public string DestinationCode { get; set; }
        public string DestinationName { get; set; }
        public string DestinationCity { get; set; }
        public string RegistrationNumber { get; set; }
        public string AircraftType { get; set; }
        public int ActiveNotifications { get; set; }
        public string WeatherAlert { get; set; }
    }

    // ==================== UPDATE ROUTE DTO ====================
    public class UpdateRouteDto
    {
        public int? Distance { get; set; }
        public int? EstimatedDuration { get; set; }
        public bool? IsActive { get; set; }
    }

    // ==================== DELETE ROUTE DTO ====================
    public class DeleteRouteDto
    {
        public string Reason { get; set; }
    }
    public class FlightNotification
    {
        public int FlightId { get; set; }
        //public bool IncludeResolved { get; set; }
    }
}
