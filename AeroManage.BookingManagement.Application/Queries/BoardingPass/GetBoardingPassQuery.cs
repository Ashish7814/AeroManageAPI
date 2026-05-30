using AeroManage.BookingManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Queries.BoardingPass
{
    public record GetBoardingPassQuery(int? BoardingPassId, string BoardingPassNumber, int? BookingPassengerId) : IRequest<ApiResponse<BoardingPassDto>>;
}
