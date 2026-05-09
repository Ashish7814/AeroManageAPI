using AeroManage.UserManagement.Domain.Entities;
using AeroMange.Shared.Repositories;
using AeroMange.UserManagement.Application.Commands.Users;
using AeroMange.UserManagement.Application.DTOs;
using AeroMange.UserManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Handlers.Users
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, ApiResponse<UserProfileDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public UpdateUserProfileCommandHandler(
            IUserRepository userRepository,
            IAuditLogRepository auditLogRepository)
        {
            _userRepository = userRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<ApiResponse<UserProfileDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(request.dto.UserId);

                if (user == null)
                {
                    return ApiResponse<UserProfileDto>.ErrorResponse("User not found");
                }

                var profile = new UserProfile
                {
                    UserId = request.dto.UserId,
                    PassportNumber = request.dto.PassportNumber,
                    PassportExpiryDate = request.dto.PassportExpiryDate,
                    Nationality = request.dto.Nationality,
                    Address = request.dto.Address,
                    City = request.dto.City,
                    State = request.dto.State,
                    Country = request.dto.Country,
                    ZipCode = request.dto.ZipCode,
                    EmergencyContactName = request.dto.EmergencyContactName,
                    EmergencyContactPhone = request.dto.EmergencyContactPhone,
                    FrequentFlyerNumber = request.dto.FrequentFlyerNumber,
                    PreferredSeatType = request.dto.PreferredSeatType,
                    MealPreference = request.dto.MealPreference,
                    ProfileImageUrl = request.dto.ProfileImageUrl
                };

                var updatedProfile = await _userRepository.UpdateUserProfileAsync(profile);

                await _auditLogRepository.CreateAuditLogAsync(
                    request.dto.UserId,
                    "PROFILE_UPDATED",
                    "UserProfiles",
                    updatedProfile.ProfileId
                );

                var profileDto = new UserProfileDto
                {
                    ProfileId = updatedProfile.ProfileId,
                    PassportNumber = updatedProfile.PassportNumber,
                    PassportExpiryDate = updatedProfile.PassportExpiryDate,
                    Nationality = updatedProfile.Nationality,
                    Address = updatedProfile.Address,
                    City = updatedProfile.City,
                    State = updatedProfile.State,
                    Country = updatedProfile.Country,
                    ZipCode = updatedProfile.ZipCode,
                    EmergencyContactName = updatedProfile.EmergencyContactName,
                    EmergencyContactPhone = updatedProfile.EmergencyContactPhone,
                    FrequentFlyerNumber = updatedProfile.FrequentFlyerNumber,
                    PreferredSeatType = updatedProfile.PreferredSeatType,
                    MealPreference = updatedProfile.MealPreference,
                    ProfileImageUrl = updatedProfile.ProfileImageUrl
                };

                return ApiResponse<UserProfileDto>.SuccessResponse(profileDto, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserProfileDto>.ErrorResponse(
                    "An error occurred while updating profile",
                    new[] { ex.Message }
                );
            }
        }
    }
}
