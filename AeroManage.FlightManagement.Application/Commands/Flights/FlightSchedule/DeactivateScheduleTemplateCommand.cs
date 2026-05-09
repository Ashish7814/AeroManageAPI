using AeroManage.FlightManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Commands.Flights.FlightSchedule
{
    public record DeactivateScheduleTemplateCommand(int TemplateId) : IRequest<ApiResponse<bool>>;
}
