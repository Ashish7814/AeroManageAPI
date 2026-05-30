using AeroMange.UserManagement.Application.Commands.Auth;
using AeroMange.UserManagement.Application.DTOs;
using AeroMange.UserManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Handlers.Auth
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, ApiResponse<bool>>
    {
        private readonly IUserRepository _userRepository;

        public VerifyEmailCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<bool>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userRepository.VerifyEmailAsync(request.dto.Token);

                if (result)
                {
                    return ApiResponse<bool>.SuccessResponse(true, "Email verified successfully");
                }

                return ApiResponse<bool>.ErrorResponse("Invalid or expired verification token");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred during email verification",
                    new[] { ex.Message }
                );
            }
        }
    }
}
