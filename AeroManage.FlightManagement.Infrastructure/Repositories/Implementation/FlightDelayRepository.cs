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
    public class FlightDelayRepository : IFlightDelayRepository
    {
        private readonly string _connectionString;

        public FlightDelayRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<Flight> ReportDelayAsync(
            int flightId,
            string delayType,
            int delayMinutes,
            string reason,
            int reportedBy)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@FlightId", flightId);
            parameters.Add("@DelayType", delayType);
            parameters.Add("@DelayMinutes", delayMinutes);
            parameters.Add("@Reason", reason);
            parameters.Add("@ReportedBy", reportedBy);

            return await connection.QueryFirstOrDefaultAsync<Flight>(
                "sp_ReportFlightDelay",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<FlightDelayReason>> GetFlightDelayHistoryAsync(int flightId)
        {
            using var connection = CreateConnection();

            return await connection.QueryAsync<FlightDelayReason>(
                "sp_GetFlightDelayHistory",
                new { FlightId = flightId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
