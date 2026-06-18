using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Interfaces
{
    public interface IBookingRepository
    {
        IDbConnection CreateConnection();
       
       
    
        Task<bool> AddMealPreferenceAsync(int bookingPassengerId, string mealType, string instructions);
        Task<bool> AddSpecialAssistanceAsync(int bookingPassengerId, string types, string details);
        Task<decimal> AddBookingAddonAsync(int bookingPassengerId, int extraBaggage, bool travelInsurance, bool priorityBoarding, bool loungeAccess);
        Task<BookingSummary> GetBookingSummaryAsync(int bookingId);
        
        //Task<bool> UpdatePassengerDetailsAsync(int passengerId, string email, string phone, string passport, DateTime? expiry);     
        
        Task<Booking> CreateBookingAsync(Booking booking, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<Booking> GetBookingByReferenceAsync(string bookingReference);
        Task<Booking> GetBookingByPNRAsync(string pnr);
        Task<(IEnumerable<Booking> Bookings, int TotalRecords)> GetUserBookingsAsync(
            int userId, int pageNumber, int pageSize);
        Task<Booking> ConfirmBookingAsync(int bookingId, string paymentIntentId);
        Task<bool> CancelBookingAsync(int bookingId, int cancelledBy, decimal refundAmount);
        Task<bool> UpdateBookingAsync(Booking booking);
        
        Task<IEnumerable<BookingFlight>> GetBookingFlightsAsync(int bookingId, CancellationToken cancellationToken = default);
        Task<(string BookingReference, string PNR)> GenerateBookingIdentifiersAsync(CancellationToken cancellationToken = default);
        //Task<bool> ProcessRefundAsync(int refundId, string stripeRefundId, string status);
      
    }
}
