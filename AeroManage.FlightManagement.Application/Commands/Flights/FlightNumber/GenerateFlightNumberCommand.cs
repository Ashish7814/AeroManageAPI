using AeroManage.FlightManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Commands.Flights.FlightNumber
{
    public record GenerateFlightNumberCommand(GenerateFlightNumberDto dto) : IRequest<ApiResponse<AeroManage.Shared.DTos.FlightNumberResultDto>>;
}
