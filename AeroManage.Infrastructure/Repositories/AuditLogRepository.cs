using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.Shared.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly string _connectionString;

        public AuditLogRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task CreateAuditLogAsync(
            int? userId,
            string action,
            string entity = null,
            int? entityId = null,
            string oldValues = null,
            string newValues = null,
            string ipAddress = null,
            string userAgent = null)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@Action", action);
            parameters.Add("@Entity", entity);
            parameters.Add("@EntityId", entityId);
            parameters.Add("@OldValues", oldValues);
            parameters.Add("@NewValues", newValues);
            parameters.Add("@IpAddress", ipAddress);
            parameters.Add("@UserAgent", userAgent);

            await connection.ExecuteAsync(
                "sp_CreateAuditLog",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
