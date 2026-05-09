using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<IEnumerable<Seat>> GetFlightSeatsAsync(int flightId);
        Task<Seat> GetSeatByIdAsync(int seatId);
        Task<Seat> GetSeatByNumberAsync(int flightId, string seatNumber);
        Task<IEnumerable<Seat>> GetAvailableSeatsByClassAsync(int flightId, string seatClass);
        Task<IEnumerable<Seat>> GetSeatMapAsync(int flightId);

        // Seat Reservation
        Task<SeatReservation> ReserveSeatAsync(int flightId, int seatId, int bookingPassengerId, int holdMinutes);
        Task<bool> ConfirmSeatReservationsAsync(int bookingId);
        Task<bool> ReleaseSeatReservationAsync(int reservationId);
        Task<bool> ReleaseExpiredReservationsAsync();
        Task<bool> IsSeatAvailableAsync(int flightId, int seatId);
    }
}
