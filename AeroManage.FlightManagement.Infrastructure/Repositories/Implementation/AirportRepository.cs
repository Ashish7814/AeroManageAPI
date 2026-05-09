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
    public class AirportRepository : IAirportRepository
    {
        private readonly string _connectionString;

        public AirportRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<Airport> CreateAirportAsync(Airport airport)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AirportCode", airport.AirportCode);
            parameters.Add("@ICAOCode", airport.ICAOCode);
            parameters.Add("@AirportName", airport.AirportName);
            parameters.Add("@City", airport.City);
            parameters.Add("@State", airport.State);
            parameters.Add("@Country", airport.Country);
            parameters.Add("@Latitude", airport.Latitude);
            parameters.Add("@Longitude", airport.Longitude);
            parameters.Add("@Timezone", airport.Timezone);

            var result = await connection.QueryFirstOrDefaultAsync<Airport>(
                "sp_CreateAirport",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<Airport> GetAirportByIdAsync(int airportId)
        {
            using var connection = CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<Airport>(
                "sp_GetAirportById",
                new { AirportId = airportId },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<(IEnumerable<Airport> Airports, int TotalRecords)> GetAllAirportsAsync(
            int pageNumber,
            int pageSize)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);

            var airports = (await connection.QueryAsync<Airport>(
                "GetAirports",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            var totalRecords = airports.FirstOrDefault()?.TotalRecords ?? 0;

            return (airports, totalRecords);
        }

    }
}
