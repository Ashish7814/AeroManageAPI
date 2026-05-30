using AeroManage.BookingManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Queries.Refund
{
    public record RequestRefundCommand(int bookingId, RefundRequestDto dto) : IRequest<ApiResponse<RefundResultDto>>;
}
