using AeroManage.BookingManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Queries.Statistics
{
    public record GetBookingStatisticsQuery(int? userId, DateTime? fromDate,  DateTime? toDate) : IRequest<BookingStatisticsDto>;
}
