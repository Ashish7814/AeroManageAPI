using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Commands.Seats
{
    public record SelectSeatCommand(int bookingId, SeatSelectionDto dto) : IRequest<ApiResponse<SeatReservation>>;
}
