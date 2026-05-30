using AeroMange.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Commands.Auth
{
    public record ChangePasswordCommand(int userId, ChangePasswordDto dto) : IRequest<ApiResponse<bool>>;
}
