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
    public class GateAssignmentRepository : IGateAssignmentRepository
    {
        private readonly string _connectionString;

        public GateAssignmentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<GateAssignment> AssignGateAsync(
            int flightId,
            string gateNumber,
            string gateType,
            DateTime scheduledTime,
            int assignedBy)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@FlightId", flightId);
            parameters.Add("@GateNumber", gateNumber);
            parameters.Add("@GateType", gateType);
            parameters.Add("@ScheduledTime", scheduledTime);
            parameters.Add("@AssignedBy", assignedBy);

            return await connection.QueryFirstOrDefaultAsync<GateAssignment>(
                "sp_AssignGate",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<Flight> ChangeFlightGateAsync(
            int flightId,
            string newGateNumber,
            string gateType,
            string reason,
            int changedBy)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@FlightId", flightId);
            parameters.Add("@NewGateNumber", newGateNumber);
            parameters.Add("@GateType", gateType);
            parameters.Add("@Reason", reason);
            parameters.Add("@ChangedBy", changedBy);

            return await connection.QueryFirstOrDefaultAsync<Flight>(
                "sp_ChangeFlightGate",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<GateAssignment>> GetFlightGateAssignmentsAsync(int flightId)
        {
            using var connection = CreateConnection();

            return await connection.QueryAsync<GateAssignment>(
                "SELECT * FROM GateAssignments WHERE FlightId = @FlightId ORDER BY AssignedAt DESC",
                new { FlightId = flightId }
            );
        }
    }
}
