using AeroMange.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Commands.Users
{
    public record UpdateUserProfileCommand(UpdateUserProfileDto dto) : IRequest<ApiResponse<UserProfileDto>>;
}
