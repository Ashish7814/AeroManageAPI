using AeroManage.BookingManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.DTOs
{
    public class FlightSearchRequestDto
    {
        [Required]
        public string TripType { get; set; } // OneWay, RoundTrip, MultiCity
        
        // For OneWay and RoundTrip
        public int? OriginAirportId { get; set; }
        public int? DestinationAirportId { get; set; }
        public DateTime? DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        
        // For MultiCity
        public List<FlightSegmentDto> Segments { get; set; }
        
        [Required, Range(1, 9)]
        public int PassengerCount { get; set; } = 1;
        
        [Required]
        public string SeatClass { get; set; } = "Economy";
        
        public FlightFiltersDto Filters { get; set; }
        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class FlightSegmentDto
    {
        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }
        public DateTime DepartureDate { get; set; }
    }

    public class FlightFiltersDto
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MaxDuration { get; set; }
        public int? MaxStops { get; set; }
        public List<int> AirlineIds { get; set; }
        public List<string> DepartureTimes { get; set; } // Morning, Afternoon, Evening, Night
        public bool DirectFlightsOnly { get; set; }
    }

    public class CalendarFaresDto
    {
        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SeatClass { get; set; } = "Economy";
    }

    public class CalendarFareResultDto
    {
        public int RouteId { get; set; }
        public DateTime FlightDate { get; set; }
        public string SeatClass { get; set; }
        public decimal MinPrice { get; set; }
        public int AvailableFlights { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class FlightDetailsDto
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public int Duration { get; set; }
        public AirportInfoDto Origin { get; set; }
        public AirportInfoDto Destination { get; set; }
        public AircraftInfoDto Aircraft { get; set; }
        public string FlightStatus { get; set; }
        public SeatAvailabilityDto SeatAvailability { get; set; }
        public PricingBreakdownDto Pricing { get; set; }
        public List<RouteLayoverDto> Layovers { get; set; }
    }

    public class AirportInfoDto
    {
        public int? AirportId { get; set; }
        public string AirportCode { get; set; } = string.Empty;
        public string AirportName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public DateTime DateTime { get; set; }
        public string Terminal { get; set; } = string.Empty;
        public string Gate { get; set; } = string.Empty;
    }

    public class AircraftInfoDto
    {
        public int AircraftId { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public int TotalSeats { get; set; }
        public Dictionary<string, int> SeatsByClass { get; set; }
    }

    public class SeatAvailabilityDto
    {
        public SeatClass seatClass { get; set; }
        public int EconomyAvailable { get; set; }
        public int BusinessAvailable { get; set; }
        public int FirstClassAvailable { get; set; }
        public int TotalAvailable { get; set; }
        public int AvailableSeats { get; set; }
    }

    /*public class PricingBreakdownDto
    {
        public decimal BasePrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; }
        public decimal EconomyPrice { get; set; }
        public decimal BusinessPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
    }*/

    public class AirlineDto
    {
        public int AirlineId { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public string Country { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ==================== BOOKING CREATION DTOs ====================
    
    public class CreateBookingRequestDto
    {
        [Required]
        public List<int> FlightIds { get; set; }
        
        [Required, MinLength(1)]
        public List<PassengerDetailsDto> Passengers { get; set; }
        public decimal TotalAmount { get; set; }

        [Required, EmailAddress]
        public string ContactEmail { get; set; }
        
        [Required, Phone]
        public string ContactPhone { get; set; }
        
        public string PromoCode { get; set; }
        public string SpecialRequests { get; set; }
        public int UserId { get; set; }
    }

    public class PassengerDetailsDto
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
        public bool TravelInsurance { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        
        [Phone]
        public string Phone { get; set; }
        
        public string FrequentFlyerNumber { get; set; }
        public BookingAddonsDto bookingAddonsDto { get; set; }
        public MealPreferenceDto mealPreferenceDto { get; set; }
        public SpecialAssistanceDto specialAssistanceDto { get; set; }
    }

    public class AddPassengerToBookingDto
    {
        [Required]
        public PassengerDetailsDto Passenger { get; set; }
    }

    public class SeatSelectionDto
    {
        [Required]
        public int PassengerId { get; set; }
        
        [Required]
        public int FlightId { get; set; }
        
        [Required]
        public string SeatCode { get; set; }
    }

    public class MealPreferenceDto
    {
        public int MealPreferenceId { get; set; }
        public int BookingPassengerId { get; set; }
        public string MealType { get; set; }
        public string SpecialInstructions { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SpecialAssistanceDto
    {
        public int AssistanceId { get; set; }
        public int PassengerId { get; set; }
        public string AssistanceType { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BookingAddonsDto
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

    // ==================== PAYMENT DTOs ====================
    
    public class InitiatePaymentDto
    {
        [Required]
        public int BookingId { get; set; }
        
        [Required]
        public string PaymentMethod { get; set; } // CreditCard, DebitCard, Wallet, UPI
        
        public string CardToken { get; set; }
        public bool SaveCard { get; set; } = false;
    }

    public class CreatePaymentDto
    {
        [Required]
        public int BookingId { get; set; }
        [Required, Range(0.01, 1000000)]
        public decimal Amount { get; set; }
        public string? Email { get; set; }
        public string? ContactName { get; set; }
        public string? PhoneNumber { get; set; }
        [Required]
        public string Currency { get; set; } = "USD";
        [Required]
        public string PaymentMethodId { get; set; } // Stripe payment method ID
        public List<string>? PaymentMethods { get; set; }
        public bool SaveCard { get; set; } = false;
    }
    public class PaymentResultDto
    {
        public bool Success { get; set; }
        public string PaymentIntentId { get; set; }
        public string PaymentStatus { get; set; }
        public string ClientSecret { get; set; } // For 3D Secure
        public string Message { get; set; }
    }

    public class ConfirmPaymentDto
    {
        [Required]
        public int BookingId { get; set; }
        
        [Required]
        public string PaymentIntentId { get; set; }
        
        public string TransactionId { get; set; }
    }

    public class PaymentStatusDto
    {
        public string PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string TransactionId { get; set; }
    }

    // ==================== BOOKING MANAGEMENT DTOs ====================
    
    public class ChangeDateDto
    {
        [Required]
        public int FlightId { get; set; }
        
        [Required]
        public DateTime NewDepartureDate { get; set; }
        
        public string Reason { get; set; }
        public int ChangedBy { get; set; }
    }

    public class UpdatePassengerDetailsDto
    {
        [Required]
        public int PassengerId { get; set; }
        
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? PassportExpiry { get; set; }
    }

    public class ChangeSeatDto
    {
        [Required]
        public int PassengerId { get; set; }
        
        [Required]
        public int FlightId { get; set; }
        
        [Required]
        public string NewSeatNumber { get; set; }
        public int ChangedBy { get; set; }
    }

    public class CancelBookingRequestDto
    {
        [Required]
        public string CancellationReason { get; set; }
        
        public bool RequestFullRefund { get; set; } = true;
        public int RequestBy { get; set; }
    }

    public class RefundRequestDto
    {
        public decimal? PartialRefundAmount { get; set; }
        public int PaymentId { get;set; }
        public string RefundReason { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public int RequestedBy { get; set; }
    }

    public class RefundResultDto
    {
        public bool Success { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal CancellationFee { get; set; }
        public string RefundStatus { get; set; }
        public string RefundReference { get; set; }
        public int ProcessingDays { get; set; }
    }

    // ==================== TICKET DTOs ====================
    
   /* public class PNRDetailsDto
    {
        public string PNR { get; set; }
        public string BookingReference { get; set; }
        public BookingDto Booking { get; set; }
        public List<PassengerTicketDto> Passengers { get; set; }
    }*/

    /*public class PassengerTicketDto
    {
        public string TicketNumber { get; set; }
        public PassengerDto Passenger { get; set; }
        public string SeatNumber { get; set; }
        public string BoardingPass { get; set; }
        public string QRCode { get; set; }
    }*/

    public class EmailTicketDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        
        public bool IncludeInvoice { get; set; } = true;
        public bool IncludeBoardingPass { get; set; } = false;
    }

    public class PrintableTicketDto
    {
        public BookingDto Booking { get; set; }
        public List<PassengerTicketDto> Passengers { get; set; }
        public List<FlightDetailsDto> Flights { get; set; }
        public string QRCode { get; set; }
        public string Barcode { get; set; }
    }

    // ==================== COMMON DTOs ====================

    /* public class BookingSummaryDto
     {
         public int BookingId { get; set; }
         public string BookingReference { get; set; }
         public string PNR { get; set; }
         public string BookingStatus { get; set; }
         public decimal TotalAmount { get; set; }
         public string Currency { get; set; }
         public DateTime BookingDate { get; set; }
         public int PassengerCount { get; set; }
         public List<FlightSummaryDto> Flights { get; set; }
         public PaymentStatusDto PaymentInfo { get; set; }
     }*/

    /*public class FlightSummaryDto
    {
        public string FlightNumber { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string OriginCode { get; set; }
        public string DestinationCode { get; set; }
        public string Status { get; set; }
    }*/

    /*public class PassengerDto
    {
        public int PassengerId { get; set; }
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? PassportExpiry { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FrequentFlyerNumber { get; set; }
    }*/

    public class PromoCodeDto
    {
        public int PromoCodeId { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinimumAmount { get; set; }
        public decimal? MaximumDiscount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; }
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

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        /*public int TotalPages =>
            PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalRecords / PageSize);*/
    }

        // ==================== BOOKING DTOs ====================

        public class BookingDto
        {
            public int BookingId { get; set; }
            public string BookingReference { get; set; }
            public List<int> FlightIds { get; set; }
            public string PromoCode { get; set; }
            public string PNR { get; set; }
            public int? UserId { get; set; }
            public string BookingStatus { get; set; }
            public decimal TotalAmount { get; set; }
            public string Currency { get; set; }
            public string PaymentStatus { get; set; }
            public DateTime BookingDate { get; set; }
            public string ContactEmail { get; set; }
            public string ContactPhone { get; set; }
            public string SpecialRequests { get; set; }
            public int PassengerCount { get; set; }
            public int FlightCount { get; set; }
            public List<BookingFlightDto> Flights { get; set; }
            public List<BookingPassengerDto> Passengers { get; set; }
        }

        public class BookingFlightDto
        {
            public int BookingFlightId { get; set; }
            public int BookingId { get; set; }
            public int FlightId { get; set; }
            public string FlightNumber { get; set; }
            public int FlightSegment { get; set; }
            public DateTime DepartureDateTime { get; set; }
            public DateTime ArrivalDateTime { get; set; }
            public string OriginCode { get; set; }
            public string DestinationCode { get; set; }
        }

        public class BookingPassengerDto
        {
            public int BookingPassengerId { get; set; }
            public int BookingId { get; set; }
            public int PassengerId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PassengerType { get; set; }
            public string SeatClass { get; set; }
            public string SeatNumber { get; set; }
            public string TicketNumber { get; set; }
            public string MealPreference { get; set; }
            public string SpecialAssistance { get; set; }
            public int ExtraBaggage { get; set; }
            public bool TravelInsurance { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string Nationality { get; set; }
            public string PassportNumber { get; set; }
            public DateTime? PassportExpiry { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string FrequentFlyerNumber { get; set; }
        }

        public class BookingPricingDto
        {
            public int PricingId { get; set; }
            public int BookingId { get; set; }
            public decimal BasePrice { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal ServiceFee { get; set; }
            public decimal BaggageFee { get; set; }
            public decimal SeatSelectionFee { get; set; }
            public decimal InsuranceFee { get; set; }
            public decimal DiscountAmount { get; set; }
            public string PromoCode { get; set; }
            public decimal TotalAmount { get; set; }
            public string Currency { get; set; }
        }

        public class BookingSummaryDto
        {
            public int BookingId { get; set; }
            public string BookingReference { get; set; }
            public string PNR { get; set; }
            public string BookingStatus { get; set; }
            public decimal TotalAmount { get; set; }
            public string Currency { get; set; }
            public string PaymentStatus { get; set; }
            public DateTime BookingDate { get; set; }
            public string ContactEmail { get; set; }
            public int PassengerCount { get; set; }
            public int FlightCount { get; set; }
            public List<FlightSummaryDto> Flights { get; set; }
            public BookingPricingDto Pricing { get; set; }
            public PaymentStatusDto PaymentInfo { get; set; }
        }

        // ==================== PASSENGER DTOs ====================

        public class PassengerDto
        {
            public int PassengerId { get; set; }
            public int? UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string Nationality { get; set; }
            public string PassportNumber { get; set; }
            public string PassengerType { get; set; }
            public DateTime? PassportExpiry { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string SeatClass { get; set; }
            public string FrequentFlyerNumber { get; set; }
        }

        public class CreatePassengerDto
        {
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

            public string PassportNumber { get; set; }
            public DateTime? PassportExpiry { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            public string Phone { get; set; }

            public string FrequentFlyerNumber { get; set; }
        }

        public class PassengerBookingDto
        {
            public CreatePassengerDto Passenger { get; set; }
            public string PassengerType { get; set; }
            public string SeatClass { get; set; }
            public string SeatNumber { get; set; }
            public string MealPreference { get; set; }
            public string SpecialAssistance { get; set; }
            public int ExtraBaggage { get; set; }
            public bool TravelInsurance { get; set; }
        }

        // ==================== PAYMENT DTOs ====================

        public class PaymentDto
        {
            public int PaymentId { get; set; }
            public int BookingId { get; set; }
            public string PaymentReference { get; set; }
            public string PaymentIntentId { get; set; }
            public string PaymentMethodId { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; }
            public string PaymentStatus { get; set; }
            public string PaymentMethod { get; set; }
            public string CardBrand { get; set; }
            public string CardLast4 { get; set; }
            public DateTime? PaymentDate { get; set; }
            public decimal? RefundAmount { get; set; }
            public DateTime? RefundDate { get; set; }
        }

       /* public class CreatePaymentDto
        {
            [Required]
            public int BookingId { get; set; }

            [Required]
            public decimal Amount { get; set; }

            [Required]
            public string Currency { get; set; } = "USD";

            public string PaymentMethodId { get; set; }
        }*/

        /*public class PaymentResultDto
        {
            public bool Success { get; set; }
            public string PaymentIntentId { get; set; }
            public string ClientSecret { get; set; }
            public string PaymentStatus { get; set; }
            public string Message { get; set; }
        }*/

       /* public class PaymentStatusDto
        {
            public string PaymentStatus { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; }
            public string PaymentMethod { get; set; }
            public DateTime? PaymentDate { get; set; }
            public string TransactionId { get; set; }
        }*/

        // ==================== SEAT DTOs ====================

        public class SeatDto
        {
            public int SeatId { get; set; }
            public int AircraftId { get; set; }
            public string SeatCode { get; set; }
            public int RowNumber { get; set; }      // 1–12
            public string Column { get; set; }
            public bool IsOccupied { get; set; }
            public bool IsAvailable => !IsOccupied;
            // Optional UI modifiers (not explicitly in UI but often needed)
            public bool IsSelected { get; set; }           // frontend state sync
            public bool IsAisle { get; set; }              // based on column gap logic
            public bool IsExitRow { get; set; }
            public bool ExtraLegroom { get; set; }
            public decimal Price { get; set; }
            public SeatAvailabilityDto availabilityDto { get; set; }
            
        }

        public class SeatMapDto
        {
            public int FlightId { get; set; }
            public List<SeatDto> Seatdto { get; set; }
        }

        public class ReserveSeatDto
        {
            [Required]
            public int FlightId { get; set; }

            [Required]
            public int SeatId { get; set; }

            [Required]
            public int BookingPassengerId { get; set; }
        }

       /* public class SeatAvailabilityDto
        {
            public int EconomyAvailable { get; set; }
            public int BusinessAvailable { get; set; }
            public int FirstClassAvailable { get; set; }
            public int TotalAvailable { get; set; }
        }*/

        // ==================== PROMO CODE DTOs ====================

        public class ValidatePromoCodeDto
        {
            [Required]
            public string Code { get; set; }

            [Required]
            public decimal BookingAmount { get; set; }
        }

        public class PromoCodeResultDto
        {
            public bool IsValid { get; set; }
            public string Code { get; set; }
            public string ValidationMessage { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal FinalAmount { get; set; }
        }

        // ==================== TICKET DTOs ====================

        public class ETicketDto
        {
            public string TicketNumber { get; set; }
            public string BookingReference { get; set; }
            public string PNR { get; set; }
            public PassengerDto Passenger { get; set; }
            public FlightDetailsDto Flight { get; set; }
            public string QRCode { get; set; }
        }

        public class InvoiceDto
        {
            public string InvoiceNumber { get; set; }
            public DateTime InvoiceDate { get; set; }
            public string BookingReference { get; set; }
            public BookingPricingDto Pricing { get; set; }
        }

        public class PNRDetailsDto
        {
            public string PNR { get; set; }
            public string BookingReference { get; set; }
            public BookingDto Booking { get; set; }
            public List<PassengerTicketDto> Passengers { get; set; }
            public List<FlightDetailsDto> Flights { get; set; }
        }

        public class PassengerTicketDto
        {
            public int BookingPassengerId { get; set; }
            public string TicketNumber { get; set; }
            public PassengerDto Passenger { get; set; }
            public string SeatNumber { get; set; }
            public string SeatClass { get; set; }
            public string BoardingPass { get; set; }
            public string QRCode { get; set; }
        }

        // ==================== FLIGHT DTOs ====================

        /*public class FlightDetailsDto
        {
            public int FlightId { get; set; }
            public string FlightNumber { get; set; }
            public DateTime DepartureDateTime { get; set; }
            public DateTime ArrivalDateTime { get; set; }
            public int Duration { get; set; }
            public string FlightStatus { get; set; }
            public AirportInfoDto Origin { get; set; }
            public AirportInfoDto Destination { get; set; }
            public AircraftInfoDto Aircraft { get; set; }
            public SeatAvailabilityDto SeatAvailability { get; set; }
            public PricingBreakdownDto Pricing { get; set; }
            public List<RouteLayoverDto> Layovers { get; set; }
        }*/

        public class FlightSummaryDto
        {
            public int FlightId { get; set; }
            public string FlightNumber { get; set; }
            public string AirlineCode { get; set; } = string.Empty;
            public string AirlineName { get; set; } = string.Empty;
            public string AircraftType { get; set; } = string.Empty;
            public string CabinClass { get; set; } = string.Empty;
            public DateTime DepartureDateTime { get; set; }
            public AirportInfoDto Departure { get; set; } = new();
            public AirportInfoDto Arrival { get; set; } = new();
            public int DurationMinutes { get; set; }
            public string Status { get; set; } = string.Empty;
            public string BaggageAllowance { get; set; } = string.Empty;
            public DateTime ArrivalDateTime { get; set; }
        }

        public class FlightSearchResultDto
        {
            public int FlightId { get; set; }
            public string FlightNumber { get; set; }
            public DateTime DepartureDateTime { get; set; }
            public DateTime ArrivalDateTime { get; set; }
            public string OriginCode { get; set; }
            public string OriginCity { get; set; }
            public string DestinationCode { get; set; }
            public string DestinationCity { get; set; }
            public string AircraftModel { get; set; }
            public decimal Price { get; set; }
            public int AvailableSeats { get; set; }
            public int Stops { get; set; }
            public int Duration { get; set; }
        }

    // ==================== AIRPORT & AIRCRAFT DTOs ====================

   /* public class AirportInfoDto
    {
        public int AirportId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Terminal { get; set; }
    }*/

    /*public class AircraftInfoDto
    {
        public int AircraftId { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public int TotalSeats { get; set; }
        public Dictionary<string, int> SeatsByClass { get; set; }
    }*/

    /*public class AirlineDto
    {
        public int AirlineId { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public string LogoUrl { get; set; }
    }
*/
    // ==================== ROUTE DTOs ====================

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

        // ==================== PRICING DTOs ====================

        public class CalculatePricingDto
        {
            public List<int> FlightIds { get; set; }
            public List<PassengerPricingDto> Passengers { get; set; }
            public string PromoCode { get; set; }
        }

        public class PassengerPricingDto
        {
            public int PassengerNumber { get; set; }
            public string PassengerType { get; set; }
            public string SeatClass { get; set; }
            public decimal BasePrice { get; set; }
            public decimal DiscountApplied { get; set; }
            public decimal FinalPrice { get; set; }
            public int ExtraBaggage { get; set; }
            public bool TravelInsurance { get; set; }
        }

        // ==================== REFUND DTOs ====================

        public class RefundDto
        {
            public int BookingId { get; set; }
            public decimal? Amount { get; set; }
            public string Reason { get; set; }
        }

        /*public class RefundResultDto
        {
            public bool Success { get; set; }
            public decimal RefundAmount { get; set; }
            public decimal CancellationFee { get; set; }
            public string RefundStatus { get; set; }
            public string RefundReference { get; set; }
            public int ProcessingDays { get; set; }
        }*/

        // ==================== MODIFICATION DTOs ====================

        public class UpdatePassengerDto
        {
            public int PassengerId { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string PassportNumber { get; set; }
            public DateTime? PassportExpiry { get; set; }
        }

        public class ModifyBookingDto
        {
            public List<UpdatePassengerDto> UpdatedPassengers { get; set; }
            public string NewContactEmail { get; set; }
            public string NewContactPhone { get; set; }
        }

        public class CancelBookingDto
        {
            [Required]
            public string CancellationReason { get; set; }
        }

        // ==================== CALENDAR FARE DTOs ====================

        /*public class CalendarFareResultDto
        {
            public DateTime Date { get; set; }
            public decimal MinPrice { get; set; }
            public int AvailableFlights { get; set; }
            public bool IsAvailable { get; set; }
        }*/

        // ==================== COMMON DTOs ====================

        

        public class FlightSearchDto
        {
            public int? OriginAirportId { get; set; }
            public int? DestinationAirportId { get; set; }
            public DateTime DepartureDate { get; set; }
            public DateTime? ReturnDate { get; set; }
            public int PassengerCount { get; set; } = 1;
            public string SeatClass { get; set; } = "Economy";
            public decimal? MaxPrice { get; set; }
            public int? MaxStops { get; set; }
            public int? AirlineId { get; set; }
            public int PageNumber { get; set; } = 1;
            public int PageSize { get; set; } = 20;
        }

    /*public class PrintableTicketDto
    {
        public BookingDto Booking { get; set; }
        public List<PassengerTicketDto> Passengers { get; set; }
        public List<FlightDetailsDto> Flights { get; set; }
        public string QRCode { get; set; }
        public string Barcode { get; set; }
    }*/



    public class BoardingPassDto
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
        public DateTime? ScannedAt { get; set; }
        public DateTime GeneratedAt { get; set; }

        // Passenger info
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string SeatNumber { get; set; }
        public string SeatClass { get; set; }

        // Booking info
        public string BookingReference { get; set; }
        public string PNR { get; set; }

        // Flight info
        public string FlightNumber { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string OriginCode { get; set; }
        public string DestinationCode { get; set; }
    }

    public class BoardingPassScanResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SeatNumber { get; set; }
        public string SeatClass { get; set; }
        public string FlightNumber { get; set; }
        public string BoardingGroup { get; set; }
        public string Gate { get; set; }
    }

    public class BoardingPassRequestDto
    {
        public int BookingPassengerId { get; set; }
        public string Gate { get; set; }
        public DateTime BoardingTime { get; set; }
        public string BoardingGroup { get; set; }
        public int? BoardingZone { get; set; }
    }
    public class BoardingPassGateRequestDto
    {
        public int FlightId { get; set; }
        public string NewGate { get; set; }
        public DateTime? NewBoardingTime { get; set; }
    }

    public class PricingCalculationRequestDto
    {
        public List<int> FlightIds { get; set; } = new();
        public List<PassengerPricingDto> Passengers { get; set; } = new();
        public int ExtraBaggageCount { get; set; }
        public bool TravelInsurance { get; set; }
        public string? PromoCode { get; set; }
    }

   

    public class PricingCalculationResponseDto
    {
        public decimal BasePrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal BaggageFee { get; set; }
        public decimal SeatSelectionFee { get; set; }
        public decimal InsuranceFee { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? PromoCode { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "USD";
        public PricingBreakdownDto Breakdown { get; set; } = new();
    }

    public class PricingBreakdownDto
    {
        public List<FlightPriceDto> FlightPrices { get; set; } = new();
        public List<PassengerPriceDto> PassengerPrices { get; set; } = new();
        public List<FeeLineItemDto> Fees { get; set; } = new();
    }

    public class FlightPriceDto
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal EconomyPrice { get; set; }
        public decimal BusinessPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
    }

    public class PassengerPriceDto
    {
        public int PassengerNumber { get; set; }
        public string PassengerType { get; set; } = string.Empty;
        public string SeatClass { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal FinalPrice { get; set; }
    }

    public class FeeLineItemDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    // ═══════════════════════════════════════════════════════════
    // BOOKING HISTORY DTOs
    // ═══════════════════════════════════════════════════════════

    public class BookingHistoryDto
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
        public FareDetailsDto? FareDetails { get; set; }
        public PaymentStatusDto? Payment { get; set; }
        public List<FlightSummaryDto> Flights { get; set; } = new();
        public PassengerDetailsDto? ContactInfo { get; set; }
        public List<PassengerSummaryDto> Passengers { get; set; } = new();
        public AuditInfoDto? AuditInfo { get; set; }
    }
    public class FareDetailsDto
    {
        public decimal BaseFare { get; set; }
        public decimal Taxes { get; set; }
        public decimal Fees { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
    }
    public class AuditInfoDto
    {
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }

    /* public class FlightSummaryDto
     {
         public string FlightNumber { get; set; } = string.Empty;
         public string OriginCode { get; set; } = string.Empty;
         public string OriginName { get; set; } = string.Empty;
         public string DestinationCode { get; set; } = string.Empty;
         public string DestinationName { get; set; } = string.Empty;
         public DateTime DepartureDateTime { get; set; }
         public DateTime ArrivalDateTime { get; set; }
     }*/

    public class PassengerSummaryDto
    {
        public int PassengerId { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
        public string PassengerType { get; set; } 
        public string SeatNumber { get; set; }
        public string TicketNumber { get; set; }
        public string? FrequentFlyerNumber { get; set; }
        public string? MealPreference { get; set; }
    }

    public class BookingHistoryFilterDto
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

    public class BookingStatisticsDto
    {
        public int TotalBookings { get; set; }
        public int ActiveBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int CompletedBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageBookingValue { get; set; }
        public int TotalPassengers { get; set; }
        public Dictionary<string, int> BookingsByStatus { get; set; } = new();
        public Dictionary<string, int> BookingsByMonth { get; set; } = new();
    }

    public class PricingResult
    {
        public decimal BasePrice { get; set; }
        public decimal Tax { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public static class BookingCacheKeys
{
    public static string Booking(int bookingId) => $"booking:{bookingId}";
}
}

