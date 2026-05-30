using AeroManage.BookingManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Commands.Passengers
{
    public record AddPassengerCommand(int bookingId, AddPassengerToBookingDto dto) : IRequest<ApiResponse<PassengerDetailsDto>>;
}
