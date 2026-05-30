using AeroMange.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Queries.Users
{
   public record GetAllUsersQuery(GetUserDto dto) : IRequest<ApiResponse<PagedResultDto<UserDto>>>;
}
