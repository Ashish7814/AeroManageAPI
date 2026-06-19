using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Interfaces
{
    public interface IPassengerRepository
    {
        Task<Passenger> CreatePassengerAsync(Passenger passenger, CancellationToken cancellationToken);
        Task<Passenger> GetPassengerByIdAsync(int passengerId, string email);
        Task<IEnumerable<Passenger>> GetPassengersByUserIdAsync(int userId);
        Task<bool> UpdatePassengerAsync(Passenger passenger);
        Task<BookingPassenger> AddPassengerToBookingAsync(BookingPassenger bookingPassenger, CancellationToken cancellationToken);
        Task<IEnumerable<BookingPassenger>> GetBookingPassengersAsync(int bookingId, CancellationToken cancellationToken);
        Task<bool> UpdateBookingPassengerAsync(BookingPassenger bookingPassenger);
    }
}
