using AeroManage.BookingManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Commands.Date
{
    public record ChangeDateCommand(int bookingId, ChangeDateDto dto) : IRequest<ApiResponse<BookingDto>>;
}
