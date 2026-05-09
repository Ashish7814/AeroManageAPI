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
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, ApiResponse<IEnumerable<Role>>>
    {
        private readonly IUserRepository _userRepository;

        public GetAllRolesQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<IEnumerable<Role>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _userRepository.GetAllRolesAsync();
                return ApiResponse<IEnumerable<Role>>.SuccessResponse(roles);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<Role>>.ErrorResponse(
                    "An error occurred while fetching roles",
                    new[] { ex.Message }
                );
            }
        }
    }
}
