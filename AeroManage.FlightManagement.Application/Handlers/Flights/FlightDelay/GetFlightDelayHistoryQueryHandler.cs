using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Flights.FlightDelay;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightDelay
{
    public class GetFlightDelayHistoryQueryHandler : IRequestHandler<GetFlightDelayHistoryQuery, ApiResponse<IEnumerable<FlightDelayReasonDto>>>
    {
        private readonly IFlightDelayRepository _repository;

        public GetFlightDelayHistoryQueryHandler(IFlightDelayRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<FlightDelayReasonDto>>> Handle(GetFlightDelayHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //var history = await _repository.GetFlightDelayHistoryAsync(request.flightId);

                //var dtos = history.Select(h => new FlightDelayReasonDto
                //{
                //    DelayId = h.DelayId,
                //    FlightId = h.FlightId,
                //    DelayType = h.DelayType,
                //    DelayMinutes = h.DelayMinutes,
                //    Reason = h.Reason,
                //    ReportedAt = h.ReportedAt,
                //    ReportedByName = h.ReportedByName
                //});

                var dtos = new List<FlightDelayReasonDto>
                {
                    new FlightDelayReasonDto
                    {
                        DelayId = 123,
                        FlightId = request.flightId,
                        DelayType= "Weather",
                        DelayMinutes= 85,
                        Reason = "Severe thunderstorm activity at JFK International, ground stop issued by FAA.",
                        ReportedAt = DateTime.Parse("2026-03-05T12:35:00"),
                        ReportedBy = new UserDto
                        {
                            UserId = 123,
                            FirstName = "Ajay",
                            LastName = " Kumar",
                            Email = "ajay@gmail.com",
                            Role = new RoleDto
                            {
                                RoleName = "Operation Manager"
                            }
                        }

                    }
                };

                return ApiResponse<IEnumerable<FlightDelayReasonDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<FlightDelayReasonDto>>.ErrorResponse("Error fetching delay history", new[] { ex.Message });
            }
        }
    }

}
