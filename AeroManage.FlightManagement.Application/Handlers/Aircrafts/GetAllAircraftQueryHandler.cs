using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Aircraft;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Aircrafts
{
    public class GetAllAircraftQueryHandler : IRequestHandler<GetAllAircraftQuery, ApiResponse<PagedResultDto<AircraftDto>>>
    {
        private readonly IAircraftRepository _repository;

        public GetAllAircraftQueryHandler(IAircraftRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<PagedResultDto<AircraftDto>>> Handle(GetAllAircraftQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //var (aircraft, totalRecords) = await _repository.GetAllAircraftAsync(
                //    request.dto.PageNumber,
                //    request.dto.PageSize,
                //    request.dto.Status);

                //var dtos = aircraft.Select(a => new AircraftDto
                //{
                //    AircraftId = a.AircraftId,
                //    RegistrationNumber = a.RegistrationNumber,
                //    AircraftType = a.AircraftType,
                //    Manufacturer = a.Manufacturer,
                //    Model = a.Model,
                //    YearManufactured = a.YearManufactured,
                //    TotalSeats = a.TotalSeats,
                //    EconomySeats = a.EconomySeats,
                //    BusinessSeats = a.BusinessSeats,
                //    FirstClassSeats = a.FirstClassSeats,
                //    CurrentStatus = a.CurrentStatus.ToString(), // Enum converted to string
                //    IsActive = a.IsActive,
                //    CurrentLocation = a.CurrentLocation,
                //    LastMaintenanceDate = a.LastMaintenanceDate,
                //    NextMaintenanceDate = a.NextMaintenanceDate,
                //    AircraftAge = a.AircraftAge
                //}).ToArray();

                //var result = new PagedResultDto<AircraftDto>
                //{
                //    PageNumber = request.dto.PageNumber,
                //    PageSize = request.dto.PageSize,
                //    TotalRecords = totalRecords,
                //    Data = dtos
                //};


                var dtos = new List<AircraftDto>
                {
                    new AircraftDto
                    {
                        AircraftId = 12343,
                        RegistrationNumber = "9V-SKB",
                        AircraftType = "Boeing 777",
                        Manufacturer = "Boeing",
                        Model = "Boeing 777-300ER",
                        YearManufactured = 2018,
                        EconomySeats = 304,
                        BusinessSeats = 64,
                        FirstClassSeats = 18,
                        TotalSeats = 386,
                        CurrentStatus = "Inflight",
                        IsActive = true,
                        CurrentLocation = "NYC",
                        LastMaintenanceDate = new DateTime(2026, 3, 10, 9, 0, 0),
                        NextMaintenanceDate = new DateTime(2026, 3, 10, 12, 0, 0),
                        AircraftAge = 4,
                        CreatedAt = new DateTime(2026, 2, 01, 9, 0, 0),
                    },
                    new AircraftDto
                    {
                        AircraftId = 12343,
                        RegistrationNumber = "A6-EVE",
                        AircraftType = "AirBus 380",
                        Manufacturer = "Airbus",
                        Model = "A380-800",
                        YearManufactured = 2016,
                        EconomySeats = 399,
                        BusinessSeats = 76,
                        FirstClassSeats = 14,
                        TotalSeats = 517,
                        CurrentStatus = "Available",
                        IsActive = true,
                        CurrentLocation = "LDN",
                        LastMaintenanceDate = new DateTime(2026, 10, 20, 9, 0, 0),
                        NextMaintenanceDate = new DateTime(2026, 4, 20, 12, 0, 0),
                        AircraftAge = 6,
                        CreatedAt = new DateTime(2026, 2, 01, 9, 0, 0),
                    },
                    new AircraftDto
                    {
                        AircraftId = 12343,
                        RegistrationNumber = "9V-SCD",
                        AircraftType = "Boeing 787",
                        Manufacturer = "Boeing",
                        Model = "Boeing 787-9",
                        YearManufactured = 2020,
                        EconomySeats = 234,
                        BusinessSeats = 48,
                        FirstClassSeats = 14,
                        CurrentStatus = "Out Of Service",
                        IsActive = true,
                        CurrentLocation = "NYC",
                        LastMaintenanceDate = new DateTime(2026, 11, 13, 9, 0, 0),
                        NextMaintenanceDate = new DateTime(2026, 3, 11, 12, 0, 0),
                        AircraftAge = 5,
                        CreatedAt = new DateTime(2026, 2, 01, 9, 0, 0),
                    },
                     new AircraftDto
                    {
                        AircraftId = 12343,
                        RegistrationNumber = "9V-SCD",
                        AircraftType = "Boeing 787",
                        Manufacturer = "Boeing",
                        Model = "Boeing 787-9",
                        YearManufactured = 2020,
                        EconomySeats = 234,
                        BusinessSeats = 48,
                        FirstClassSeats = 14,
                        CurrentStatus = "Maintenance",
                        IsActive = true,
                        CurrentLocation = "NYC",
                        LastMaintenanceDate = new DateTime(2026, 11, 13, 9, 0, 0),
                        NextMaintenanceDate = new DateTime(2026, 3, 11, 12, 0, 0),
                        AircraftAge = 5,
                        CreatedAt = new DateTime(2026, 2, 01, 9, 0, 0),
                    }
                };
                int pageNumber = request?.dto?.PageNumber ?? 1;
                int pageSize = request?.dto?.PageSize ?? 10;

                var pagedData = dtos
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var result = new PagedResultDto<AircraftDto>
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = dtos.Count,
                    Data = pagedData
                };

                //return ApiResponse<PagedResultDto<AircraftDto>>.SuccessResponse(result);
                return ApiResponse<PagedResultDto<AircraftDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResultDto<AircraftDto>>.ErrorResponse("Error fetching aircraft", new[] { ex.Message });
            }
        }
    }
}
