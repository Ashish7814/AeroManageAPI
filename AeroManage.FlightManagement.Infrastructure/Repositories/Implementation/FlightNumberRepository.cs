using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.DTos;
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
    public class FlightNumberRepository : IFlightNumberRepository
    {
        private readonly string _connectionString;

        public FlightNumberRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<FlightNumberResultDto> GenerateFlightNumberAsync(string prefix)
        {
            using var connection = CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<FlightNumberResultDto>(
                "sp_GenerateFlightNumber",
                new { Prefix = prefix },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
