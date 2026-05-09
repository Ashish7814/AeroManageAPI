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
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ApiResponse<PagedResultDto<UserDto>>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<PagedResultDto<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var (users, totalRecords) = await _userRepository.GetAllUsersAsync(
                    request.dto.PageNumber,
                    request.dto.PageSize,
                    request.dto.SearchTerm,
                    request.dto.RoleId
                );

                var userDtos = users.Select(MapToUserDto).ToArray();

                var pagedResult = new PagedResultDto<UserDto>
                {
                    PageNumber = request.dto.PageNumber,
                    PageSize = request.dto.PageSize,
                    TotalRecords = totalRecords,
                    Data = userDtos
                };

                return ApiResponse<PagedResultDto<UserDto>>.SuccessResponse(pagedResult);
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResultDto<UserDto>>.ErrorResponse(
                    "An error occurred while fetching users",
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
