using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Airports;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Airports
{
    public class GetAllAirportsQueryHandler : IRequestHandler<GetAllAirportsQuery, ApiResponse<PagedResultDto<AirportDto>>>
    {
        private readonly IAirportRepository _repository;

        public GetAllAirportsQueryHandler(IAirportRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<PagedResultDto<AirportDto>>> Handle(GetAllAirportsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //var (airports, totalRecords) = await _repository.GetAllAirportsAsync(
                //    request.dto.PageNumber,
                //    request.dto.PageSize
                //    request.dto.SearchTerm);

                //var dtos = airports.Select(a => new AirportDto
                //{
                //    AirportId = a.AirportId,
                //    AirportCode = a.AirportCode,
                //    ICAOCode = a.ICAOCode,
                //    AirportName = a.AirportName,
                //    City = a.City,
                //    State = a.State,
                //    Country = a.Country,
                //    Latitude = a.Latitude,
                //    Longitude = a.Longitude,
                //    Timezone = a.Timezone
                //}).ToArray();

                var dtos = new List<AirportDto>
                {
                    new AirportDto
                    {
                        AirportId = 1,
                        AirportCode = "SIN",
                        AirportName = "Changi Airport",
                        ICAOCode = "WSSS",
                        City = "Singapore",
                        State = "",
                        Country = "Singapore",
                        Latitude = 35.754543564m,
                        Longitude = 103.9915m,
                        Timezone = "UTC+8",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 3, 10, 9, 0, 0)
                    },
                    new AirportDto
                    {
                        AirportId = 2,
                        AirportCode = "DXB",
                        AirportName = "Dubai International Airport",
                        ICAOCode = "OMDB",
                        City = "Dubai",
                        State = "Dubai",
                        Country = "UAE",
                        Latitude = 35.754543564m,
                        Longitude = 55.754543564m,
                        Timezone = "UTC+4",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 10, 10, 9, 0, 0)
                    },
                    new AirportDto
                    {
                        AirportId = 3,
                        AirportCode = "LHR",
                        AirportName = "London Heathrow Airport",
                        ICAOCode = "WSSS",
                        City = "London",
                        State = "England",
                        Country = "United Kingdom",
                        Latitude = 51.754543564m,
                        Longitude = 35.754543564m,
                        Timezone = "UTC+0",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 3, 10, 9, 0, 0)
                    },
                    new AirportDto
                    {
                        AirportId = 4,
                        AirportCode = "JFK",
                        AirportName = "John F. Kennedy Intl Airport",
                        ICAOCode = "KJFK",
                        City = "New York",
                        State = "New York",
                        Country = "USA",
                        Latitude = 35.754543564m,
                        Longitude = 35.754543564m,
                        Timezone = "UTC-5",
                        IsActive = true,
                        CreatedAt = new DateTime(2026, 3, 10, 9, 0, 0)
                    },
                    new AirportDto
                    {
                        AirportId = 4,
                        AirportCode = "CDG",
                        AirportName = "Charles de Gaulle Airport",
                        ICAOCode = "LFPG",
                        City = "Paris",
                        State = "Île-de-France",
                        Country = "France",
                        Latitude = 35.754543564m,
                        Longitude = 35.754543564m,
                        Timezone = "UTC-5",
                        IsActive = false,
                        Regions = "Asia Pasific",
                        CreatedAt = new DateTime(2026, 3, 10, 9, 0, 0)
                    },

                };

                int pageNumber = request?.dto?.PageNumber ?? 1;
                int pageSize = request?.dto?.PageSize ?? 10;

                var pagedData = dtos
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var result = new PagedResultDto<AirportDto>
                {
                    PageNumber = request.dto.PageNumber,
                    PageSize = request.dto.PageSize,
                    TotalRecords = dtos.Count,
                    Data = dtos
                };

                return ApiResponse<PagedResultDto<AirportDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResultDto<AirportDto>>.ErrorResponse("Error fetching airports", new[] { ex.Message });
            }
        }
    }
}
