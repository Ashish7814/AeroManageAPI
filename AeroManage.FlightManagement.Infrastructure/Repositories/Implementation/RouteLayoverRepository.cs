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
    public class RouteLayoverRepository : IRouteLayoverRepository
    {
        private readonly string _connectionString;

        public RouteLayoverRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<RouteLayover> AddLayoverAsync(int routeId, int airportId, int layoverSequence, int minMinutes, int maxMinutes)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@RouteId", routeId);
            parameters.Add("@AirportId", airportId);
            parameters.Add("@LayoverSequence", layoverSequence);
            parameters.Add("@MinimumLayoverMinutes", minMinutes);
            parameters.Add("@MaximumLayoverMinutes", maxMinutes);

            return await connection.QueryFirstOrDefaultAsync<RouteLayover>(
                "sp_AddRouteLayover",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<RouteLayover>> GetRouteLayoversAsync(int routeId)
        {
            using var connection = CreateConnection();

            // Get route info and layovers
            using var multi = await connection.QueryMultipleAsync(
                "sp_GetRouteWithLayovers",
                new { RouteId = routeId },
                commandType: CommandType.StoredProcedure
            );

            // Skip route details (first result set)
            await multi.ReadAsync();

            // Get layovers (second result set)
            return await multi.ReadAsync<RouteLayover>();
        }

        public async Task<bool> DeleteLayoverAsync(int layoverId)
        {
            using var connection = CreateConnection();

            var affected = await connection.ExecuteAsync(
                "UPDATE RouteLayovers SET IsActive = 0 WHERE LayoverId = @LayoverId",
                new { LayoverId = layoverId }
            );

            return affected > 0;
        }
    }

}
