using AeroManage.UserManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Service
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal ValidateToken(string token);
        int? GetUserIdFromToken(string token);
    }
}
