using AeroManage.UserManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Infrastructure.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<int> SaveRefreshTokenAsync(int userId, string token, DateTime expiresAt);
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
        Task RevokeAllUserTokensAsync(int userId);
    }
}
