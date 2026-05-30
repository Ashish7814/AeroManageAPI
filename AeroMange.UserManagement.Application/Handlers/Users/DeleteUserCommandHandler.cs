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
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public DeleteUserCommandHandler(
            IUserRepository userRepository,
            IAuditLogRepository auditLogRepository)
        {
            _userRepository = userRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userRepository.DeleteUserAsync(request.userId);

                if (!result)
                {
                    return ApiResponse<bool>.ErrorResponse("User not found or already deleted");
                }

                await _auditLogRepository.CreateAuditLogAsync(
                    request.userId,
                    "USER_DELETED",
                    "Users",
                    request.userId
                );

                return ApiResponse<bool>.SuccessResponse(true, "User deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred while deleting user",
                    new[] { ex.Message }
                );
            }
        }
    }
}
