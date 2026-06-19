using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Domain.Interfaces;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Implementation
{
    public class SeatRepository : ISeatRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;
        private readonly ILogger<SeatRepository> _logger;

        public SeatRepository(IDapperUnitOfWork unitOfWork, ILogger<SeatRepository> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<Seat>> GetFlightSeatsAsync(int flightId)
        {
            var seats = await _unitOfWork.Connection.QueryAsync<Seat>(
                "sp_GetFlightSeats",
                new { FlightId = flightId },
                commandType: CommandType.StoredProcedure
            );

            return seats;
        }

        // ==================== GET SEAT BY ID ====================

        public async Task<Seat> GetSeatByIdAsync(int seatId)
        {
            var sql = @"
                SELECT SeatId, AircraftId, SeatNumber, SeatClass, SeatType,
                       IsAvailable, IsExitRow, ExtraLegroom, Price, CreatedAt
                FROM Seats
                WHERE SeatId = @SeatId";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Seat>(sql, new { SeatId = seatId });
        }

        // ==================== GET SEAT BY NUMBER ====================

        public async Task<Seat> GetSeatByNumberAsync(int flightId, string seatNumber)
        {
            var sql = @"
                SELECT s.SeatId, s.AircraftId, s.SeatNumber, s.SeatClass, s.SeatType,
                       s.IsAvailable, s.IsExitRow, s.ExtraLegroom, s.Price
                FROM Seats s
                INNER JOIN Flights f ON s.AircraftId = f.AircraftId
                WHERE f.FlightId = @FlightId
                  AND s.SeatNumber = @SeatNumber";

            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Seat>(
                sql, new { FlightId = flightId, SeatNumber = seatNumber }
            );
        }

        // ==================== GET AVAILABLE SEATS BY CLASS ====================

        public async Task<IEnumerable<Seat>> GetAvailableSeatsByClassAsync(int flightId, string seatClass)
        {
            var sql = @"
                SELECT s.SeatId, s.AircraftId, s.SeatNumber, s.SeatClass, s.SeatType,
                       s.IsAvailable, s.IsExitRow, s.ExtraLegroom, s.Price
                FROM Seats s
                INNER JOIN Flights f ON s.AircraftId = f.AircraftId
                WHERE f.FlightId = @FlightId
                  AND s.SeatClass = @SeatClass
                  AND s.IsAvailable = 1
                  AND NOT EXISTS (
                      SELECT 1 FROM SeatReservations sr
                      WHERE sr.SeatId = s.SeatId
                        AND sr.FlightId = @FlightId
                        AND sr.ReservationStatus IN ('Reserved', 'Confirmed')
                  )
                ORDER BY s.SeatNumber";

            return await _unitOfWork.Connection.QueryAsync<Seat>(
                sql, new { FlightId = flightId, SeatClass = seatClass }
            );
        }

        // ==================== RESERVE SEAT ====================

        public async Task<SeatReservation> ReserveSeatAsync(
            int flightId, int seatId, int bookingPassengerId, int holdMinutes)
        {
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_ReserveSeat",
                new
                {
                    FlightId = flightId,
                    SeatId = seatId,
                    BookingPassengerId = bookingPassengerId,
                    HoldMinutes = holdMinutes
                },
                commandType: CommandType.StoredProcedure
            );

            if (result == null)
            {
                throw new InvalidOperationException("Failed to reserve seat");
            }

            return new SeatReservation
            {
                ReservationId = (int)result.ReservationId,
                FlightId = flightId,
                SeatId = seatId,
                BookingPassengerId = bookingPassengerId,
                ReservationStatus = result.ReservationStatus,
                ExpiresAt = result.ExpiresAt,
                CreatedAt = DateTime.UtcNow
            };
        }

        // ==================== CONFIRM SEAT RESERVATIONS ====================

        public async Task<bool> ConfirmSeatReservationsAsync(int bookingId)
        {
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_ConfirmSeatReservations",
                new { BookingId = bookingId },
                commandType: CommandType.StoredProcedure
            );

            var confirmedCount = (int)(result?.ConfirmedCount ?? 0);

            _logger.LogInformation($"Confirmed {confirmedCount} seat reservations for booking {bookingId}");

            return confirmedCount >= 0;
        }

        // ==================== RELEASE SEAT RESERVATION ====================

        public async Task<bool> ReleaseSeatReservationAsync(int reservationId)
        {
            var sql = @"
                UPDATE SeatReservations
                SET ReservationStatus = 'Released'
                WHERE ReservationId = @ReservationId
                  AND ReservationStatus IN ('Reserved', 'Hold')";

            var rows = await _unitOfWork.Connection.ExecuteAsync(sql, new { ReservationId = reservationId });

            return rows > 0;
        }

        // ==================== RELEASE EXPIRED RESERVATIONS ====================

        public async Task<bool> ReleaseExpiredReservationsAsync()
        {
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_ReleaseExpiredReservations",
                commandType: CommandType.StoredProcedure
            );

            var releasedCount = (int)(result?.ReleasedCount ?? 0);

            _logger.LogInformation($"Released {releasedCount} expired seat reservations");

            return true;
        }

        // ==================== IS SEAT AVAILABLE ====================

        public async Task<bool> IsSeatAvailableAsync(int flightId, int seatId)
        {
            var sql = @"
                SELECT CASE
                    WHEN EXISTS (
                        SELECT 1 FROM SeatReservations
                        WHERE SeatId = @SeatId
                          AND FlightId = @FlightId
                          AND ReservationStatus IN ('Reserved', 'Confirmed')
                    ) THEN 0
                    WHEN EXISTS (
                        SELECT 1 FROM Seats
                        WHERE SeatId = @SeatId AND IsAvailable = 1
                    ) THEN 1
                    ELSE 0
                END AS IsAvailable";

            var isAvailable = await _unitOfWork.Connection.ExecuteScalarAsync<int>(
                sql, new { FlightId = flightId, SeatId = seatId }
            );

            return isAvailable == 1;
        }

        // ==================== GET SEAT MAP ====================

        public async Task<IEnumerable<Seat>> GetSeatMapAsync(int flightId)
        {
            var sql = @"
                SELECT
                    s.SeatId,
                    s.SeatNumber,
                    s.SeatClass,
                    s.SeatType,
                    s.IsExitRow,
                    s.ExtraLegroom,
                    s.Price,
                    CASE
                        WHEN sr.SeatId IS NOT NULL THEN 0
                        ELSE s.IsAvailable
                    END AS IsAvailable
                FROM Seats s
                INNER JOIN Flights f ON s.AircraftId = f.AircraftId
                LEFT JOIN SeatReservations sr
                    ON sr.SeatId = s.SeatId
                    AND sr.FlightId = @FlightId
                    AND sr.ReservationStatus IN ('Reserved', 'Confirmed')
                WHERE f.FlightId = @FlightId
                ORDER BY s.SeatClass, s.SeatNumber";

            return await _unitOfWork.Connection.QueryAsync<Seat>(
                sql, new { FlightId = flightId }
            );
        }
        public async Task<bool> ChangeSeatAsync(int bookingPassengerId, int flightId, string newSeat, int changedBy)
        {
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_ChangeSeat",
                new
                {
                    BookingPassengerId = bookingPassengerId,
                    FlightId = flightId,
                    NewSeatNumber = newSeat,
                    ChangedBy = changedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return result != null && result.Success == 1;
        }

        public async Task<SeatAvailability> GetSeatAvailabilityAsync(int flightId, string seatClass)
        {
            try
            {
                var results = await _unitOfWork.Connection.QueryAsync<dynamic>(
                    "sp_GetSeatAvailability",
                    new { FlightId = flightId, SeatClass = seatClass },
                    commandType: CommandType.StoredProcedure
                );

                var availability = new SeatAvailability();
                foreach (var row in results)
                {
                    if (row.SeatClass == "Economy")
                    {
                        availability.EconomyAvailable = row.AvailableSeats;
                    }
                    else if (row.SeatClass == "Business")
                    {
                        availability.BusinessAvailable = row.AvailableSeats;
                    }
                    else if (row.SeatClass == "FirstClass")
                    {
                        availability.FirstClassAvailable = row.AvailableSeats;
                    }
                    availability.AvailableSeats += (int)row.AvailableSeats;
                }

                return availability;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
