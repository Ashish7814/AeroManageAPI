using AeroManage.BookingManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Queries.Seats
{
    public record GetAvailableSeatsByClassQuery(int flightId, string seatClass) : IRequest<ApiResponse<List<SeatDto>>>;
}
