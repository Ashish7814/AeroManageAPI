using AeroManage.UserManagement.Domain.Entities;
using AeroMange.UserManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroMange.UserManagement.Application.Queries.Users
{
    public record GetAllRolesQuery : IRequest<ApiResponse<IEnumerable<Role>>>;
}
