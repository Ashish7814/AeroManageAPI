using AeroManage.UserManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user, string emailVerificationToken, System.DateTime tokenExpiry);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task<User> UpdateUserAsync(User user);
        Task<UserProfile> UpdateUserProfileAsync(UserProfile profile);
        Task<bool> VerifyEmailAsync(string token);
        Task<bool> SetPasswordResetTokenAsync(string email, string token, System.DateTime expiry);
        Task<bool> ResetPasswordAsync(string token, string passwordHash, string passwordSalt);
        Task UpdateLastLoginAsync(int userId);
        Task<(IEnumerable<User> Users, int TotalRecords)> GetAllUsersAsync(int pageNumber, int pageSize, string searchTerm, int? roleId);
        Task<bool> DeleteUserAsync(int userId);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<bool> EmailExistsAsync(string email);
    }
}
