using AeroManage.UserManagement.Domain.Entities;
using AeroMange.UserManagement.Domain.Interfaces;
using AeroMange.UserManagement.Infrastructure.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Infrastructure.Repositories.Implemention
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public RefreshTokenRepository(IDapperUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }

        public async Task<int> SaveRefreshTokenAsync(int userId, string token, DateTime expiresAt)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@Token", token);
            parameters.Add("@ExpiresAt", expiresAt);

            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_SaveRefreshToken",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result?.TokenId ?? 0;
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<RefreshToken>(
                "sp_GetRefreshToken",
                new { Token = token },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "sp_RevokeRefreshToken",
                new { Token = token },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task RevokeAllUserTokensAsync(int userId)
        {
            var query = @"UPDATE RefreshTokens 
                         SET IsRevoked = 1, RevokedAt = GETDATE() 
                         WHERE UserId = @UserId AND IsRevoked = 0";

            await _unitOfWork.Connection.ExecuteAsync(query, new { UserId = userId });
        }
    }
}
