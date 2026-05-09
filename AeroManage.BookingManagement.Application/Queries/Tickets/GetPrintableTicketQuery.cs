using AeroManage.BookingManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Queries.Tickets
{
    public record GetPrintableTicketQuery(int bookingId)  : IRequest<ApiResponse<PrintableTicketDto>>;
}
