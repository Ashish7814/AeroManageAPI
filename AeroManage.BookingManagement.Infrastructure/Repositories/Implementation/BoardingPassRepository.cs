using AeroManage.BookingManagement.Domain.Entities;
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
    public class BoardingPassRepository : IBoardingPassRepository
    {
        private readonly string _connectionString;

        public BoardingPassRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<(int BoardingPassId, string BoardingPassNumber, string QRCode, string BoardingGroup, int BoardingZone)>
            GenerateBoardingPassAsync(int bookingPassengerId, string gate, DateTime boardingTime, string boardingGroup = null, int? boardingZone = null)
        {
            using var connection = CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_GenerateBoardingPass",
                new
                {
                    BookingPassengerId = bookingPassengerId,
                    Gate = gate,
                    BoardingTime = boardingTime,
                    BoardingGroup = boardingGroup,
                    BoardingZone = boardingZone
                },
                commandType: CommandType.StoredProcedure
            );

            return (result.BoardingPassId, result.BoardingPassNumber, result.QRCode, result.BoardingGroup ?? "", result.BoardingZone ?? 0);
        }

        public async Task<BoardingPass> GetBoardingPassAsync(int? boardingPassId, string boardingPassNumber, int? bookingPassengerId)
        {
            using var connection = CreateConnection();

            var boardingPass = await connection.QueryFirstOrDefaultAsync<BoardingPass>(
                "sp_GetBoardingPass",
                new
                {
                    BoardingPassId = boardingPassId,
                    BoardingPassNumber = boardingPassNumber,
                    BookingPassengerId = bookingPassengerId
                },
                commandType: CommandType.StoredProcedure
            );

            return boardingPass;
        }

        public async Task<BoardingPassScan> ScanBoardingPassAsync(string boardingPassNumber, string scannedBy)
        {
            using var connection = CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<BoardingPassScan>(
                "sp_ScanBoardingPass",
                new
                {
                    BoardingPassNumber = boardingPassNumber,
                    ScannedBy = scannedBy
                },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<List<BoardingPass>> GetFlightBoardingPassesAsync(int flightId)
        {
            using var connection = CreateConnection();

            var passes = await connection.QueryAsync<BoardingPass>(
                "sp_GetFlightBoardingPasses",
                new { FlightId = flightId },
                commandType: CommandType.StoredProcedure
            );

            return passes.AsList();
        }

        public async Task<int> UpdateBoardingPassGateAsync(int flightId, string newGate, DateTime? newBoardingTime)
        {
            using var connection = CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_UpdateBoardingPassGate",
                new
                {
                    FlightId = flightId,
                    NewGate = newGate,
                    NewBoardingTime = newBoardingTime
                },
                commandType: CommandType.StoredProcedure
            );

            return result?.PassengersUpdated ?? 0;
        }
    }
}
