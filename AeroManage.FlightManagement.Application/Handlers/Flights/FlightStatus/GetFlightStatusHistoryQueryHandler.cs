using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Flights.FlightStatus;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightStatus
{
    public class GetFlightStatusHistoryQueryHandler : IRequestHandler<GetFlightStatusHistoryQuery, ApiResponse<IEnumerable<FlightStatusHistoryDto>>>
    {
        private readonly IFlightRepository _repository;

        public GetFlightStatusHistoryQueryHandler(IFlightRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<FlightStatusHistoryDto>>> Handle(GetFlightStatusHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //var history = await _repository.GetFlightStatusHistoryAsync(request.flightId);

                //var dtos = history.Select(h => new FlightStatusHistoryDto
                //{
                //    StatusHistoryId = h.StatusHistoryId,
                //    FlightId = h.FlightId,
                //    OldStatus = h.OldStatus,
                //    NewStatus = h.NewStatus,
                //    Reason = h.Reason,
                //    ChangedBy = h.ChangedBy,
                //    ChangedAt = h.ChangedAt
                //});
                var dtos = new List<FlightStatusHistoryDto>
                {
                    new FlightStatusHistoryDto
                    {
                        StatusHistoryId = 1,
                        FlightId = 20241,
                        OldStatus = "Boarding",
                        NewStatus = "InFlight",
                        Reason = "Aircraft airborne, all systems nominal",
                        ChangedAt = DateTime.Parse("2026-03-05T10:08:00"),
                        ChangedBy = new UserDto
                        {
                            UserId = 1,
                            FirstName = "System",
                            LastName = "Auto",
                            Email = "system@airline.com"
                        }
                    },
                    new FlightStatusHistoryDto
                    {
                        StatusHistoryId = 2,
                        FlightId = 20241,
                        OldStatus = "Scheduled",
                        NewStatus = "Boarding",
                        Reason = "Passenger boarding commenced gate A12",
                        ChangedAt = DateTime.Parse("2026-03-05T09:50:00"),
                        ChangedBy = new UserDto
                        {
                            UserId = 2,
                            FirstName = "Ground",
                            LastName = "Control",
                            Email = "ground.control@airline.com"
                        }
                    },
                    new FlightStatusHistoryDto
                    {
                        StatusHistoryId = 3,
                        FlightId = 20241,
                        OldStatus = "Scheduled",
                        NewStatus = null,
                        Reason = "Flight scheduled as per template SIN-DXB-001",
                        ChangedAt = DateTime.Parse("2026-02-24T15:00:00"),
                        ChangedBy = new UserDto
                        {
                            UserId = 3,
                            FirstName = "Arjun",
                            LastName = "Kumar",
                            Email = "arjun.kumar@airline.com"
                        }
                    }
                };
                return ApiResponse<IEnumerable<FlightStatusHistoryDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<FlightStatusHistoryDto>>.ErrorResponse("Error fetching status history", new[] { ex.Message });
            }
        }
    }
}
