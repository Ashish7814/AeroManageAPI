using AeroManage.BookingManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Queries.Histories
{
    public record GetBookingHistoryByIdQuery(BookingHistoryFilterDto dto) : IRequest<(List<BookingHistoryDto> Bookings, int TotalCount)>;
}
