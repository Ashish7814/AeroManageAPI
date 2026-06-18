using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Interfaces
{
    public interface IFlightDetailsRepository
    {
        Task<FlightDetails> GetFlightDetailsAsync(int flightId);
        Task<SeatAvailability> GetSeatAvailabilityAsync(int flightId, string seatClass);
        Task<bool> ChangeFlightDateAsync(int bookingId, int flightId, DateTime newDate, int changedBy, string reason);
        Task<bool> AddFlightToBookingAsync(int bookingId, int flightId, int flightSegment, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
    }
}
