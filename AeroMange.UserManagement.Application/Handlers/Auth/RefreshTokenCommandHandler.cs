using AeroManage.UserManagement.Domain.Entities;
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
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly int _refreshTokenExpirationDays;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtTokenService jwtTokenService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenService = jwtTokenService;
            _refreshTokenExpirationDays = int.Parse(configuration["Jwt:RefreshTokenExpirationDays"]);
        }

        public async Task<ApiResponse<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var storedToken = await _refreshTokenRepository.GetRefreshTokenAsync(request.dto.RefreshToken);

                if (storedToken == null || !storedToken.IsActive)
                {
                    return ApiResponse<AuthResponseDto>.ErrorResponse("Invalid or expired refresh token");
                }

                var user = await _userRepository.GetUserByIdAsync(storedToken.UserId);

                if (user == null || !user.IsActive)
                {
                    return ApiResponse<AuthResponseDto>.ErrorResponse("User not found or inactive");
                }

                // Revoke old token
                await _refreshTokenRepository.RevokeRefreshTokenAsync(request.dto.RefreshToken);

                // Generate new tokens
                var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
                var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
                var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

                await _refreshTokenRepository.SaveRefreshTokenAsync(
                    user.UserId,
                    newRefreshToken,
                    newRefreshTokenExpiry
                );

                var response = new AuthResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                    User = MapToUserDto(user)
                };

                return ApiResponse<AuthResponseDto>.SuccessResponse(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse(
                    "An error occurred during token refresh",
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
