using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces
{
    public interface IBookingPricingRepository
    {
        Task<BookingPricing> CreatePricingAsync(BookingPricing pricing);
        Task<BookingPricing> GetPricingByBookingIdAsync(int bookingId);
        Task<decimal> CalculateTotalPriceAsync(
           List<int> flightIds,
           List<(string PassengerType, string SeatClass, int ExtraBaggage, bool TravelInsurance)> passengers,
           string promoCode = null);
        Task<bool> UpdatePricingAsync(BookingPricing pricing);
        Task<ClaculatedPrice> CalculatePricingAsync(PricingCalculation request);
        Task<(List<BookingHistory> Bookings, int TotalCount)> GetBookingHistoryAsync(BookingHistoryFilter filter);
        Task<BookingStatistics> GetBookingStatisticsAsync(int? userId = null, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
