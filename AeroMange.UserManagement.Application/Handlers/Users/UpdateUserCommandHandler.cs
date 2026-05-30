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
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public UpdateUserCommandHandler(
            IUserRepository userRepository,
            IAuditLogRepository auditLogRepository)
        {
            _userRepository = userRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<ApiResponse<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByIdAsync(request.dto.UserId);

                if (existingUser == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("User not found");
                }

                existingUser.FirstName = request.dto.FirstName;
                existingUser.LastName = request.dto.LastName;
                existingUser.PhoneNumber = request.dto.PhoneNumber;
                existingUser.DateOfBirth = request.dto.DateOfBirth;
                existingUser.Gender = request.dto.Gender;

                var updatedUser = await _userRepository.UpdateUserAsync(existingUser);

                await _auditLogRepository.CreateAuditLogAsync(
                    request.dto.UserId,
                    "USER_UPDATED",
                    "Users",
                    request.dto.UserId
                );

                var userDto = MapToUserDto(updatedUser);
                return ApiResponse<UserDto>.SuccessResponse(userDto, "User updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse(
                    "An error occurred while updating user",
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
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}
