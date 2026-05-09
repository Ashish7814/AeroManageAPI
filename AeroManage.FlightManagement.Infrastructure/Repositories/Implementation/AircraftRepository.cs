using AeroManage.FlightManagement.Domain.Entities;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Implementation
{
    public class AircraftRepository : IAircraftRepository
    {
        private readonly string _connectionString;

        public AircraftRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<Aircraft> CreateAircraftAsync(Aircraft aircraft)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@RegistrationNumber", aircraft.RegistrationNumber);
            parameters.Add("@AircraftType", aircraft.AircraftType);
            parameters.Add("@Manufacturer", aircraft.Manufacturer);
            parameters.Add("@Model", aircraft.Model);
            parameters.Add("@YearManufactured", aircraft.YearManufactured);
            parameters.Add("@TotalSeats", aircraft.TotalSeats);
            parameters.Add("@EconomySeats", aircraft.EconomySeats);
            parameters.Add("@BusinessSeats", aircraft.BusinessSeats);
            parameters.Add("@FirstClassSeats", aircraft.FirstClassSeats);

            var result = await connection.QueryFirstOrDefaultAsync<Aircraft>(
                "sp_CreateAircraft",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<Aircraft> GetAircraftByIdAsync(int? aircraftId)
        {
            using var connection = CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<Aircraft>(
                "sp_GetAircraftById",
                new { AircraftId = aircraftId },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<(IEnumerable<Aircraft> Aircraft, int TotalRecords)> GetAllAircraftAsync(
            int pageNumber,
            int pageSize,
            string status)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@Status", status);

            var aircraft = (await connection.QueryAsync<Aircraft>(
                "sp_GetAllAircraft",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            var totalRecords = aircraft.FirstOrDefault()?.TotalRecords ?? 0;

            return (aircraft, totalRecords);
        }

        public async Task<Aircraft> UpdateAircraftStatusAsync(int aircraftId, string status)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AircraftId", aircraftId);
            parameters.Add("@Status", status);

            var result = await connection.QueryFirstOrDefaultAsync<Aircraft>(
                "sp_UpdateAircraftStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

    }
}
