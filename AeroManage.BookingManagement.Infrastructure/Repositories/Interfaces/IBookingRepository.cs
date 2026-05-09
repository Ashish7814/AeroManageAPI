using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        IDbConnection CreateConnection();
        Task<List<CalendarFare>> GetCalendarFaresAsync(int originId, int destId, DateTime start, DateTime end, string seatClass);
        Task<FlightDetails> GetFlightDetailsAsync(int flightId);
        Task<SeatAvailability> GetSeatAvailabilityAsync(int flightId, string seatClass);
        Task<List<Airline>> GetActiveAirlinesAsync();
        Task<bool> AddMealPreferenceAsync(int bookingPassengerId, string mealType, string instructions);
        Task<bool> AddSpecialAssistanceAsync(int bookingPassengerId, string types, string details);
        Task<decimal> AddBookingAddonAsync(int bookingPassengerId, int extraBaggage, bool travelInsurance, bool priorityBoarding, bool loungeAccess);
        Task<BookingSummary> GetBookingSummaryAsync(int bookingId);
        Task<bool> ChangeFlightDateAsync(int bookingId, int flightId, DateTime newDate, int changedBy, string reason);
        Task<bool> UpdatePassengerDetailsAsync(int passengerId, string email, string phone, string passport, DateTime? expiry);
        Task<decimal> CalculateCancellationFeeAsync(int bookingId);
        Task<(int RefundId, string RefundReference)> CreateRefundRequestAsync(int bookingId, int paymentId, decimal refundAmount, decimal cancellationFee,
            string reason, string bankAccount, string bankName, int requestedBy);
        Task<bool> ChangeSeatAsync(int bookingPassengerId, int flightId, string newSeat, int changedBy);
        Task<Booking> CreateBookingAsync(Booking booking, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<Booking> GetBookingByReferenceAsync(string bookingReference);
        Task<Booking> GetBookingByPNRAsync(string pnr);
        Task<(IEnumerable<Booking> Bookings, int TotalRecords)> GetUserBookingsAsync(
            int userId, int pageNumber, int pageSize);
        Task<Booking> ConfirmBookingAsync(int bookingId, string paymentIntentId);
        Task<bool> CancelBookingAsync(int bookingId, int cancelledBy, decimal refundAmount);
        Task<bool> UpdateBookingAsync(Booking booking);
        Task<bool> AddFlightToBookingAsync(int bookingId, int flightId, int flightSegment, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<IEnumerable<BookingFlight>> GetBookingFlightsAsync(int bookingId, CancellationToken cancellationToken = default);
        Task<(string BookingReference, string PNR)> GenerateBookingIdentifiersAsync(CancellationToken cancellationToken = default);
        Task<bool> ProcessRefundAsync(int refundId, string stripeRefundId, string status);
        Task<BookingPricing> CreatePricingAsync(BookingPricing pricing, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    }
}
