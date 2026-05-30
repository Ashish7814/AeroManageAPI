using AeroManage.UserManagement.Domain.Entities;
using AeroMange.UserManagement.Application.DTOs;
using AeroMange.UserManagement.Application.Queries.Users;
using AeroMange.UserManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Handlers.Users
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ApiResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(request.userId);

                if (user == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("User not found");
                }

                var userDto = MapToUserDto(user);
                return ApiResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse(
                    "An error occurred while fetching user",
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
                RoleName = user.Roles.RoleName,
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
