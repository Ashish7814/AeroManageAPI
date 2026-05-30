using AeroManage.UserManagement.Domain.Entities;
using AeroMange.Shared.Repositories;
using AeroMange.UserManagement.Application.Commands;
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
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly int _refreshTokenExpirationDays;

        public RegisterUserCommandHandler(
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

        public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if email already exists
                if (await _userRepository.EmailExistsAsync(request.dto.Email))
                {
                    return ApiResponse<AuthResponseDto>.ErrorResponse("Email already exists");
                }

                // Hash password
                var (hash, salt) = PasswordHasher.HashPassword(request.dto.Password);

                // Generate email verification token
                var verificationToken = Guid.NewGuid().ToString();
                var tokenExpiry = DateTime.UtcNow.AddHours(24);

                // Create user
                var user = new User
                {
                    Email = request.dto.Email,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    FirstName = request.dto.FirstName,
                    LastName = request.dto.LastName,
                    PhoneNumber = request.dto.PhoneNumber,
                    DateOfBirth = request.dto.DateOfBirth,
                    Gender = request.dto.Gender,
                    RoleId = request.dto.RoleId
                };

                var createdUser = await _userRepository.CreateUserAsync(user, verificationToken, tokenExpiry);

                if (createdUser == null)
                {
                    return ApiResponse<AuthResponseDto>.ErrorResponse("Failed to create user");
                }

                // Create audit log
                await _auditLogRepository.CreateAuditLogAsync(
                    createdUser.UserId,
                    "USER_REGISTERED",
                    "Users",
                    createdUser.UserId
                );

                // Generate tokens
                var accessToken = _jwtTokenService.GenerateAccessToken(createdUser);
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                var refreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);

                await _refreshTokenRepository.SaveRefreshTokenAsync(
                    createdUser.UserId,
                    refreshToken,
                    refreshTokenExpiry
                );

                var response = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                    User = MapToUserDto(createdUser)
                };

                return ApiResponse<AuthResponseDto>.SuccessResponse(
                    response,
                    "User registered successfully. Please verify your email."
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<AuthResponseDto>.ErrorResponse(
                    "An error occurred during registration",
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
