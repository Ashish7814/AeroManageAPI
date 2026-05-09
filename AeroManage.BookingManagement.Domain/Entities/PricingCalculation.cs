using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class PricingCalculation
    {
        public List<int> FlightIds { get; set; } = new();
        public List<PassengerPricing> Passengers { get; set; } = new();
        public int ExtraBaggageCount { get; set; }
        public bool TravelInsurance { get; set; }
        public string? PromoCode { get; set; }
    }
    public class PassengerPricing
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
    public class ClaculatedPrice
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
        public PricingBreakdown Breakdown { get; set; } = new();
    }
    public class PricingBreakdown
    {
        public List<FlightPrice> FlightPrices { get; set; } = new();
        public List<PassengerPrice> PassengerPrices { get; set; } = new();
        public List<FeeLineItem> Fees { get; set; } = new();
    }
    public class FlightPrice
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal EconomyPrice { get; set; }
        public decimal BusinessPrice { get; set; }
        public decimal FirstClassPrice { get; set; }
    }

    public class PassengerPrice
    {
        public int PassengerNumber { get; set; }
        public string PassengerType { get; set; } = string.Empty;
        public string SeatClass { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal FinalPrice { get; set; }

    }
}
