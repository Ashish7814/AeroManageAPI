using AeroManage.BookingManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Commands.Promocode
{
    public record UpdatePromoCodeCommand(PromoCodeDto dto) : IRequest<ApiResponse<bool>>;
}
