using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Flights.FlightCrew;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.UserManagement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights.FlightCrew
{
    public class GetFlightCrewQueryHandler : IRequestHandler<GetFlightCrewQuery, ApiResponse<IEnumerable<FlightCrewDto>>>
    {
        private readonly IFlightRepository _repository;

        public GetFlightCrewQueryHandler(IFlightRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<FlightCrewDto>>> Handle(GetFlightCrewQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //var crew = await _repository.GetFlightCrewAsync(request.flightId);

                //var dtos = crew.Select(c => new FlightCrewDto
                //{
                //    FlightCrewId = c.FlightCrewId,
                //    FlightId = c.FlightId,
                //    UserId = c.UserId,

                //    CrewRole = c.users.Roles?.RoleName,

                //    AssignedAt = c.AssignedAt,

                //    FirstName = c.users?.FirstName,
                //    LastName = c.users?.LastName,
                //    Email = c.users?.Email,
                //    PhoneNumber = c.users?.PhoneNumber
                //});

                var crew = new List<FlightCrewDto>
                {
                    new FlightCrewDto
                    {
                        FlightCrewId = 1,
                        FlightId = request.flightId,
                        UserId = 101,
                        AssignedAt = DateTime.UtcNow,
                        Users = new CrewUserDto
                        {
                            FirstName = "Richard",
                            LastName = "Williams",
                            Email = "richard.williams@airline.com",
                            PhoneNumber = "+1 555 123 456",
                            Roles = new RoleDto
                            {
                                RoleName = "Pilot"
                            }
                        }
                    },
                    new FlightCrewDto
                    {
                        FlightCrewId = 2,
                        FlightId = request.flightId,
                        UserId = 102,
                        AssignedAt = DateTime.UtcNow,
                        Users = new CrewUserDto
                        {
                            FirstName = "Sarah",
                            LastName = "Lin",
                            Email = "sarah.lin@airline.com",
                            PhoneNumber = "+1 555 888 222",
                            Roles = new RoleDto
                            {
                                RoleName = "Co-Pilot"
                            }
                        }
                    },
                    new FlightCrewDto
                    {
                        FlightCrewId = 3,
                        FlightId = request.flightId,
                        UserId = 103,
                        AssignedAt = DateTime.UtcNow,
                        Users = new CrewUserDto
                        {
                            FirstName = "Arjun",
                            LastName = "Mehta",
                            Email = "arjun.mehta@airline.com",
                            PhoneNumber = "+1 555 444 111",
                            Roles = new RoleDto
                            {
                                RoleName = "Cabin Crew"
                            }
                        }
                    }
                };

                //return ApiResponse<IEnumerable<FlightCrewDto>>.SuccessResponse(dtos);
                return ApiResponse<IEnumerable<FlightCrewDto>>.SuccessResponse(crew);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<FlightCrewDto>>.ErrorResponse("Error fetching flight crew", new[] { ex.Message });
            }
        }
    }
}
