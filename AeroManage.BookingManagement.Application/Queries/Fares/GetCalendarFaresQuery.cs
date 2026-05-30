using AeroManage.BookingManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Queries.Fares
{
    public record GetCalendarFaresQuery(CalendarFaresDto dto) : IRequest<ApiResponse<List<CalendarFareResultDto>>>;
}
