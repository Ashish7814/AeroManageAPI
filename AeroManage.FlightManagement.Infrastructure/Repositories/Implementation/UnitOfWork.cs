using AeroManage.FlightManagement.Domain.Interfaces;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
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
    public class UnitOfWork : IUnitOfWork, IDapperUnitOfWork
    {
        private readonly string _connectionString;
        private SqlConnection? _connection;
        private IDbTransaction? _transaction;

        public IDbConnection Connection =>
           _connection ?? throw new InvalidOperationException("Call BeginAsync first.");
        public IDbTransaction? Transaction => _transaction;

        public UnitOfWork(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }


        public async Task BeginAsync(CancellationToken cancellationToken = default)
        {
            _connection = new SqlConnection(_connectionString);
            await _connection.OpenAsync(cancellationToken);
            _transaction = _connection.BeginTransaction();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            Transaction?.Commit();
            return Task.CompletedTask;
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            Transaction?.Rollback();
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            Transaction?.Dispose();
            if (Connection is SqlConnection sqlConn)
                await sqlConn.DisposeAsync();
            else
                Connection?.Dispose();
        }
    }
}
