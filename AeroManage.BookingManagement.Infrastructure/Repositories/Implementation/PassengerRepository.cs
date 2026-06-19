using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Domain.Interfaces;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Implementation
{
    public class PassengerRepository : IPassengerRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;
        public PassengerRepository(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Passenger> CreatePassengerAsync(Passenger passenger, CancellationToken cancellationToken = default)
        {
            //using var connection = CreateConnection();

            //var sql = @"
            //    INSERT INTO Passengers (
            //        UserId, FirstName, LastName, DateOfBirth, Gender, Nationality,
            //        PassportNumber, PassportExpiry, Email, Phone, FrequentFlyerNumber
            //    )
            //    VALUES (
            //        @UserId, @FirstName, @LastName, @DateOfBirth, @Gender, @Nationality,
            //        @PassportNumber, @PassportExpiry, @Email, @Phone, @FrequentFlyerNumber
            //    );
            //    SELECT CAST(SCOPE_IDENTITY() as int);";

            //var passengerId = await connection.ExecuteScalarAsync<int>(sql, passenger);
            //passenger.PassengerId = passengerId;

            //return passenger;

            const string sql = @"
                INSERT INTO Passengers (
                    UserId, FirstName, LastName, DateOfBirth, Gender, Nationality,
                    PassportNumber, PassportExpiry, Email, Phone, FrequentFlyerNumber, CreatedAt
                )
                VALUES (
                    @UserId, @FirstName, @LastName, @DateOfBirth, @Gender, @Nationality,
                    @PassportNumber, @PassportExpiry, @Email, @Phone, @FrequentFlyerNumber, @CreatedAt
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var passengerId = await _unitOfWork.Connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, passenger, 
                    cancellationToken: cancellationToken));

            passenger.PassengerId = passengerId;
            return passenger;
        }

        public async Task<Passenger> GetPassengerByIdAsync(int passengerId, string email)
        {
            var sql = @"
                SELECT 
                    PassengerId, UserId, FirstName, LastName, DateOfBirth, Gender, Nationality,
                    PassportNumber, PassportExpiry, Email, Phone, FrequentFlyerNumber,
                    CreatedAt, UpdatedAt
                FROM Passengers
                WHERE PassengerId = @PassengerId";

            var passenger = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Passenger>(sql, new { PassengerId = passengerId });

            return passenger;
        }
        public async Task<IEnumerable<Passenger>> GetPassengersByUserIdAsync(int userId)
        {
            var sql = @"
                SELECT 
                    PassengerId, UserId, FirstName, LastName, DateOfBirth, Gender, Nationality,
                    PassportNumber, PassportExpiry, Email, Phone, FrequentFlyerNumber,
                    CreatedAt, UpdatedAt
                FROM Passengers
                WHERE UserId = @UserId
                ORDER BY CreatedAt DESC";

            var passengers = await _unitOfWork.Connection.QueryAsync<Passenger>(sql, new { UserId = userId });

            return passengers;
        }
        public async Task<bool> UpdatePassengerAsync(Passenger passenger)
        {
            var sql = @"
                UPDATE Passengers
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    Phone = @Phone,
                    PassportNumber = @PassportNumber,
                    PassportExpiry = @PassportExpiry,
                    UpdatedAt = GETDATE()
                WHERE PassengerId = @PassengerId";

            var rowsAffected = await _unitOfWork.Connection.ExecuteAsync(sql, passenger);

            return rowsAffected > 0;
        }

        public async Task<BookingPassenger> AddPassengerToBookingAsync(BookingPassenger bookingPassenger,  CancellationToken cancellationToken = default)
        {
            //using var connection = CreateConnection();

            //var result = await connection.QueryFirstOrDefaultAsync<int>(
            //    "sp_AddPassengerToBooking",
            //    new
            //    {
            //        BookingId = bookingPassenger.BookingId,
            //        PassengerId = bookingPassenger.PassengerId,
            //        PassengerType = bookingPassenger.PassengerType,
            //        SeatClass = bookingPassenger.SeatClass,
            //        MealPreference = bookingPassenger.MealPreference,
            //        SpecialAssistance = bookingPassenger.SpecialAssistance,
            //        ExtraBaggage = bookingPassenger.ExtraBaggage
            //    },
            //    commandType: CommandType.StoredProcedure
            //);

            //bookingPassenger.BookingPassengerId = result;

            //return bookingPassenger;

            var id = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<int>(
               new CommandDefinition(
                   "sp_AddPassengerToBooking",
           new
           {
               bookingPassenger.BookingId,
               bookingPassenger.PassengerId,
               bookingPassenger.PassengerType,
               bookingPassenger.SeatClass,
               bookingPassenger.MealPreference,
               bookingPassenger.SpecialAssistance,
               bookingPassenger.ExtraBaggage,
               bookingPassenger.TravelInsurance   // ← now included
           },
           commandType: CommandType.StoredProcedure,
           cancellationToken: cancellationToken));

            bookingPassenger.BookingPassengerId = id;
            return bookingPassenger;

        }

        public async Task<IEnumerable<BookingPassenger>> GetBookingPassengersAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            //using var connection = CreateConnection();

            //var sql = @"
            //    SELECT 
            //        bp.BookingPassengerId,
            //        bp.BookingId,
            //        bp.PassengerId,
            //        bp.PassengerType,
            //        bp.SeatClass,
            //        bp.SeatNumber,
            //        bp.MealPreference,
            //        bp.SpecialAssistance,
            //        bp.ExtraBaggage,
            //        bp.TicketNumber,
            //        bp.ETicketPath,
            //        bp.QRCodePath,
            //        p.FirstName,
            //        p.LastName,
            //        p.Email,
            //        p.Phone
            //    FROM BookingPassengers bp
            //    INNER JOIN Passengers p ON bp.PassengerId = p.PassengerId
            //    WHERE bp.BookingId = @BookingId";

            //var passengers = await connection.QueryAsync<BookingPassenger>(sql, new { BookingId = bookingId });

            //return passengers;

            const string sql = @"
                SELECT p.PassengerId, p.FirstName, p.LastName, p.Email,
                       bp.PassengerType, bp.SeatClass, bp.MealPreference,
                       bp.ExtraBaggage, bp.TravelInsurance
                FROM   BookingPassengers bp
                INNER JOIN Passengers p ON p.PassengerId = bp.PassengerId
                WHERE  bp.BookingId = @BookingId";

            return await _unitOfWork.Connection.QueryAsync<BookingPassenger>(
                new CommandDefinition(sql, new { BookingId = bookingId },
                    cancellationToken: cancellationToken));
        }
        public async Task<bool> UpdateBookingPassengerAsync(BookingPassenger bookingPassenger)
        {
            var sql = @"
                UPDATE BookingPassengers
                SET SeatNumber = @SeatNumber,
                    MealPreference = @MealPreference,
                    SpecialAssistance = @SpecialAssistance,
                    ExtraBaggage = @ExtraBaggage,
                    TicketNumber = @TicketNumber,
                    ETicketPath = @ETicketPath,
                    QRCodePath = @QRCodePath
                WHERE BookingPassengerId = @BookingPassengerId";

            var rowsAffected = await _unitOfWork.Connection.ExecuteAsync(sql, bookingPassenger);

            return rowsAffected > 0;
        }
    }


}

