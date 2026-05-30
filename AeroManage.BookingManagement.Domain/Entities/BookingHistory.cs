using AeroManage.BookingManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class BookingHistory
    {
        public int BookingId { get; set; }
        public string BookingReference { get; set; }
        public string PNR { get; set; }
        public string BookingStatus { get; set; }
        public string BookingChannel { get; set; } = string.Empty;
        public string BookingSource { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public DateTime? TravelDate { get; set; }
        public int PassengerCount { get; set; }
        public int FlightCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "USD";
        public FareDetails? FareDetails { get; set; }
        public Payment Payment { get; set; }
        public List<FlightSummary> Flights { get; set; } = new();
        public PassengerDetails? ContactInfo { get; set; }
        public List<PassengerSummary> Passengers { get; set; } = new();
        public AuditInfo? AuditInfo { get; set; }
    }

    public class AuditInfo
    {
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }

    public class PassengerDetails
    {
        public int PassengerId { get; set; }
        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Nationality { get; set; }

        [Required]
        public string PassengerType { get; set; } // Adult, Child, Infant

        [Required]
        public string SeatClass { get; set; }

        public string PassportNumber { get; set; }
        public DateTime? PassportExpiry { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string FrequentFlyerNumber { get; set; }
        public BookingAddons? bookingAddons { get; set; }
        public MealPreference? mealPreference { get; set; }
        public SpecialAssistance? specialAssistance { get; set; }
    }
    public class FareDetails
    {
        public decimal BaseFare { get; set; }
        public decimal Taxes { get; set; }
        public decimal Fees { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
    }

    public class BookingHistoryFilter
    {
        public int? UserId { get; set; }
        public string? BookingReference { get; set; }
        public string? PNR { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class BookingAddons
    {
        [Required]
        public int PassengerId { get; set; }

        [Range(0, 50)]
        public int ExtraBaggage { get; set; } = 0;

        public bool TravelInsurance { get; set; } = false;

        public bool PriorityBoarding { get; set; } = false;

        public bool LoungeAccess { get; set; } = false;

        public List<string> OtherServices { get; set; }
    }

}
