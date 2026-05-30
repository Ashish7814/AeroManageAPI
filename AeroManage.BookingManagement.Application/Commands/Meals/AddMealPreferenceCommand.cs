using AeroManage.BookingManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Commands.Meals
{
    public record AddMealPreferenceCommand(int bookingId, MealPreferenceDto dto) : IRequest<ApiResponse<bool>>;
}
