using AeroManage.BookingManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Queries.Bookings
{
    public record GetUserBookingsQuery(int userId, int pageNumber = 1, int pageSize = 10) : IRequest<ApiResponse<PagedResultDto<BookingDto>>>;
}
