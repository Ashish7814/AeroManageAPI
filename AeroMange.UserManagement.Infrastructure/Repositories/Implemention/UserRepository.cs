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
    public class UserRepository : IUserRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public UserRepository(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User> CreateUserAsync(User user, string emailVerificationToken, DateTime tokenExpiry)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", user.Email);
            parameters.Add("@PasswordHash", user.PasswordHash);
            parameters.Add("@PasswordSalt", user.PasswordSalt);
            parameters.Add("@FirstName", user.FirstName);
            parameters.Add("@LastName", user.LastName);
            parameters.Add("@PhoneNumber", user.PhoneNumber);
            parameters.Add("@DateOfBirth", user.DateOfBirth);
            parameters.Add("@Gender", user.Gender);
            parameters.Add("@RoleId", user.RoleId);
            parameters.Add("@EmailVerificationToken", emailVerificationToken);
            parameters.Add("@EmailVerificationTokenExpiry", tokenExpiry);

            var result = await _unitOfWork.Connection.QueryAsync<User, UserProfile, User>(
                "sp_CreateUser",
                (user, profile) =>
                {
                    user.Profile = profile;
                    return user;
                },
                parameters,
                splitOn: "ProfileId",
                commandType: CommandType.StoredProcedure
            );

            return result.FirstOrDefault();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var userDictionary = new Dictionary<int, User>();

            var result = await _unitOfWork.Connection.QueryAsync<User, UserProfile, User>(
                "sp_GetUserByEmail",
                (user, profile) =>
                {
                    if (!userDictionary.TryGetValue(user.UserId, out var userEntry))
                    {
                        userEntry = user;
                        userEntry.Profile = profile;
                        userDictionary.Add(userEntry.UserId, userEntry);
                    }
                    return userEntry;
                },
                new { Email = email },
                splitOn: "ProfileId",
                commandType: CommandType.StoredProcedure
            );

            return userDictionary.Values.FirstOrDefault();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var userDictionary = new Dictionary<int, User>();

            var result = await _unitOfWork.Connection.QueryAsync<User, UserProfile, User>(
                "sp_GetUserById",
                (user, profile) =>
                {
                    if (!userDictionary.TryGetValue(user.UserId, out var userEntry))
                    {
                        userEntry = user;
                        userEntry.Profile = profile;
                        userDictionary.Add(userEntry.UserId, userEntry);
                    }
                    return userEntry;
                },
                new { UserId = userId },
                splitOn: "ProfileId",
                commandType: CommandType.StoredProcedure
            );

            return userDictionary.Values.FirstOrDefault();
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", user.UserId);
            parameters.Add("@FirstName", user.FirstName);
            parameters.Add("@LastName", user.LastName);
            parameters.Add("@PhoneNumber", user.PhoneNumber);
            parameters.Add("@DateOfBirth", user.DateOfBirth);
            parameters.Add("@Gender", user.Gender);

            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(
                "sp_UpdateUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<UserProfile> UpdateUserProfileAsync(UserProfile profile)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", profile.UserId);
            parameters.Add("@PassportNumber", profile.PassportNumber);
            parameters.Add("@PassportExpiryDate", profile.PassportExpiryDate);
            parameters.Add("@Nationality", profile.Nationality);
            parameters.Add("@Address", profile.Address);
            parameters.Add("@City", profile.City);
            parameters.Add("@State", profile.State);
            parameters.Add("@Country", profile.Country);
            parameters.Add("@ZipCode", profile.ZipCode);
            parameters.Add("@EmergencyContactName", profile.EmergencyContactName);
            parameters.Add("@EmergencyContactPhone", profile.EmergencyContactPhone);
            parameters.Add("@FrequentFlyerNumber", profile.FrequentFlyerNumber);
            parameters.Add("@PreferredSeatType", profile.PreferredSeatType);
            parameters.Add("@MealPreference", profile.MealPreference);
            parameters.Add("@ProfileImageUrl", profile.ProfileImageUrl);

            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<UserProfile>(
                "sp_UpdateUserProfile",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_VerifyEmail",
                new { Token = token },
                commandType: CommandType.StoredProcedure
            );

            return result?.Success == 1;
        }

        public async Task<bool> SetPasswordResetTokenAsync(string email, string token, DateTime expiry)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);
            parameters.Add("@Token", token);
            parameters.Add("@Expiry", expiry);

            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_SetPasswordResetToken",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result?.Success == 1;
        }

        public async Task<bool> ResetPasswordAsync(string token, string passwordHash, string passwordSalt)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Token", token);
            parameters.Add("@PasswordHash", passwordHash);
            parameters.Add("@PasswordSalt", passwordSalt);

            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_ResetPassword",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result?.Success == 1;
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "sp_UpdateLastLogin",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<(IEnumerable<User> Users, int TotalRecords)> GetAllUsersAsync(
            int pageNumber,
            int pageSize,
            string searchTerm,
            int? roleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@SearchTerm", searchTerm);
            parameters.Add("@RoleId", roleId);

            var users = (await _unitOfWork.Connection.QueryAsync<Users>(
                "sp_GetAllUsers",
                parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();

            var totalRecords = users.FirstOrDefault()?.TotalRecords ?? 0;

            return (users, totalRecords);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var result = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<dynamic>(
                "sp_DeleteUser",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            );

            return result?.Success > 0;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _unitOfWork.Connection.QueryAsync<Role>(
                "sp_GetAllRoles",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var query = "SELECT COUNT(1) FROM Users WHERE Email = @Email AND IsActive = 1";
            var count = await _unitOfWork.Connection.ExecuteScalarAsync<int>(query, new { Email = email });

            return count > 0;
        }
    }

    // Add this property to User entity for paging
    public partial class Users : User
    {
        public int TotalRecords { get; set; }
    }
}
