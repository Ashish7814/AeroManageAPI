using AeroManage.FlightManagement.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Queries.Flights
{
    public record SearchFlightsQuery(FlightSearchDto dto) : IRequest<ApiResponse<PagedResultDto<FlightDto>>>;
}
