using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces
{
    public interface IPassengerRepository
    {
        IDbConnection CreateConnection();
        Task<Passenger> CreatePassengerAsync(Passenger passenger, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken);
        Task<Passenger> GetPassengerByIdAsync(int passengerId, string email);
        Task<IEnumerable<Passenger>> GetPassengersByUserIdAsync(int userId);
        Task<bool> UpdatePassengerAsync(Passenger passenger);
        Task<BookingPassenger> AddPassengerToBookingAsync(BookingPassenger bookingPassenger, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken);
        Task<IEnumerable<BookingPassenger>> GetBookingPassengersAsync(int bookingId, CancellationToken cancellationToken);
        Task<bool> UpdateBookingPassengerAsync(BookingPassenger bookingPassenger);
    }
}
