using AeroManage.UserManagement.Domain.Entities;
using AeroMange.Shared.Repositories;
using AeroMange.UserManagement.Application.Commands.Auth;
using AeroMange.UserManagement.Application.DTOs;
using AeroMange.UserManagement.Application.Service;
using AeroMange.UserManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Handlers.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly int _refreshTokenExpirationDays;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IAuditLogRepository auditLogRepository,
            IJwtTokenService jwtTokenService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _auditLogRepository = auditLogRepository;
            _jwtTokenService = jwtTokenService;
            _refreshTokenExpirationDays = int.Parse(configuration["Jwt:RefreshTokenExpirationDays"]);
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(request.dto.Email);

                if (user == null)
                {
                    return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");
                }

                // Verify password
                if (!PasswordHasher.VerifyPassword(request.dto.Password, user.PasswordHash, user.PasswordSalt))
                {
                    await _auditLogRepository.CreateAuditLogAsync(
                        user.UserId,
                        "LOGIN_FAILED",
                        "Users",
                        user.UserId
                    );
                    return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid email or password");
                }

                // Update last login
                await _userRepository.UpdateLastLoginAsync(user.UserId);

                // Generate tokens
                var accessToken = _jwtTokenService.GenerateAccessToken(user);
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

                await _refreshTokenRepository.SaveRefreshTokenAsync(
                    user.UserId,
                    refreshToken,
                    refreshTokenExpiry
                );

                // Create audit log
                await _auditLogRepository.CreateAuditLogAsync(
                    user.UserId,
                    "LOGIN_SUCCESS",
                    "Users",
                    user.UserId
                );

                var response = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                    User = MapToUserDto(user)
                };

                return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse(
                    "An error occurred during login",
                    new[] { ex.Message }
                );
            }
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                //RoleName = user.RoleName,
                IsEmailVerified = user.IsEmailVerified,
                LastLoginAt = user.LastLoginAt,
                Profile = user.Profile != null ? new UserProfileDto
                {
                    ProfileId = user.Profile.ProfileId,
                    PassportNumber = user.Profile.PassportNumber,
                    PassportExpiryDate = user.Profile.PassportExpiryDate,
                    Nationality = user.Profile.Nationality,
                    Address = user.Profile.Address,
                    City = user.Profile.City,
                    State = user.Profile.State,
                    Country = user.Profile.Country,
                    ZipCode = user.Profile.ZipCode,
                    EmergencyContactName = user.Profile.EmergencyContactName,
                    EmergencyContactPhone = user.Profile.EmergencyContactPhone,
                    FrequentFlyerNumber = user.Profile.FrequentFlyerNumber,
                    PreferredSeatType = user.Profile.PreferredSeatType,
                    MealPreference = user.Profile.MealPreference,
                    ProfileImageUrl = user.Profile.ProfileImageUrl
                } : null
            };
        }
    }
}
