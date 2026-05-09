using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.FlightSchedule;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Templates
{
    public class GetAllScheduleTemplatesQueryHandler : IRequestHandler<GetAllScheduleTemplatesQuery, ApiResponse<IEnumerable<FlightScheduleTemplateDto>>>
    {
        private readonly IFlightScheduleTemplateRepository _repository;

        public GetAllScheduleTemplatesQueryHandler(IFlightScheduleTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<FlightScheduleTemplateDto>>> Handle(GetAllScheduleTemplatesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //var (templates, totalRecords) = await _repository.GetAllTemplatesAsync(
                //    request.pageNumber,
                //    request.pageSize,
                //    request.isActive
                //);

                //var dtos = templates.Select(t => new FlightScheduleTemplateDto
                //{
                //    TemplateId = t.Id,
                //    TemplateName = t.TemplateName,
                //    FlightNumberPrefix = t.FlightNumberPrefix,
                //    StartDate = t.StartDate,
                //    EndDate = t.EndDate,
                //    DepartureTime = t.DepartureTime,
                //    ArrivalTime = t.ArrivalTime,
                //    IsActive = t.IsActive,
                //    Route = new RouteDto
                //    {
                //        RouteId = t.RouteId,
                //        RouteCode = t.Route?.RouteCode,
                //        Distance = t.Route.Distance,
                //        EstimatedDuration = t.Route.EstimatedDuration,
                //        OriginAirport = t.Route.OriginAirport == null ? null : new AirportDto
                //        {
                //            AirportId = t.Route.OriginAirport.AirportId,
                //            AirportCode = t.Route.OriginAirport.AirportCode,
                //            City = t.Route.OriginAirport.City,
                //            AirportName = t.Route.OriginAirport.AirportName,
                //            State = t.Route.OriginAirport.State,
                //            Country = t.Route.OriginAirport.Country,
                //            Longitude = t.Route.OriginAirport.Longitude,
                //            Latitude = t.Route.OriginAirport.Latitude,
                //            Timezone = t.Route.OriginAirport.Timezone
                //        },
                //        DestinationAirport = t.Route.DestinationAirport == null ? null : new AirportDto
                //        {
                //            AirportId = t.Route.DestinationAirport.AirportId,
                //            AirportCode = t.Route.DestinationAirport.AirportCode,
                //            City = t.Route.DestinationAirport.City,
                //            AirportName = t.Route.OriginAirport.AirportName,
                //            State = t.Route.OriginAirport.State,
                //            Country = t.Route.OriginAirport.Country,
                //            Longitude = t.Route.OriginAirport.Longitude,
                //            Latitude = t.Route.OriginAirport.Latitude,
                //            Timezone = t.Route.OriginAirport.Timezone
                //        }
                //    },
                //    Aircraft = t.Aircraft == null ? null : new AircraftDto
                //    {
                //        AircraftId = t.Aircraft.AircraftId,
                //        RegistrationNumber = t.Aircraft.RegistrationNumber,
                //        AircraftType = t.Aircraft.AircraftType,
                //        Manufacturer = t.Aircraft.Manufacturer,
                //        Model = t.Aircraft.Model,
                //        TotalSeats = t.Aircraft.TotalSeats,
                //        EconomySeats = t.Aircraft.EconomySeats,
                //        BusinessSeats = t.Aircraft.BusinessSeats,
                //        FirstClassSeats = t.Aircraft.FirstClassSeats,
                //        CurrentStatus = t.Aircraft.CurrentStatus,
                //        LastMaintenanceDate = t.Aircraft.LastMaintenanceDate,
                //        NextMaintenanceDate = t.Aircraft.NextMaintenanceDate,
                //        IsActive = t.Aircraft.IsActive
                //    },
                //    Recurrence = t.Recurrence == null ? null : new RecurrenceDto
                //    {
                //        RecurrenceType = t.Recurrence.RecurrenceType,
                //        DaysOfWeek = t.Recurrence.Days?.Select(d => d.DayOfWeek).ToList(),
                //        DayOfMonth = t.Recurrence.Monthly?.DayOfMonth
                //    },
                //    Pricings = t.Pricings?.Select(p => new FlightPriceDto
                //    {
                //        ClassType = p.ClassType,
                //        FlightId = p.FlightId,
                //        Price = p.Price
                //    }).ToList()
                //}).ToArray();
                var dtos = new List<FlightScheduleTemplateDto>
                {
                    new FlightScheduleTemplateDto
                    {
                         TemplateId = 123,
                    TemplateName = "Morning Express",
                    FlightNumberPrefix = "AM · B777-300ER",
                    StartDate = DateTime.Parse("Jan 01 2026"),
                    EndDate = DateTime.Parse("Dec 31 2026"),
                    DepartureTime = TimeSpan.Parse("09:45"),
                    ArrivalTime = TimeSpan.Parse("17:30"),
                    IsActive = true,
                    Route = new RouteDto
                    {
                        RouteId = 342,
                        RouteCode = "SIN–DXB",
                        Distance = 2000,
                        EstimatedDuration = 290,
                        //OriginAirport = Route.OriginAirport == null ? null : new AirportDto
                        OriginAirport =  new AirportDto
                        {
                            AirportId = 3456,
                            AirportCode = "SIN",
                            City = "Santadr",
                            AirportName = "tspgptp",
                            State = "slfbobfg",
                            Country = "sbfosbdo",
                            Latitude = 35.754543564m,
                            Longitude = 24.305983684m,
                            Timezone = "US_Central"
                        },
                        //DestinationAirport = t.Route.DestinationAirport == null ? null : new AirportDto
                        DestinationAirport = new AirportDto
                        {
                            AirportId = 2423,
                            AirportCode = "DXB",
                            City = "fdlbf",
                            AirportName = "dfoubos",
                            State = "dlhbsob",
                            Country = "dfobosr",
                            Latitude = 45.983578m,
                            Longitude = 45.2582588m,
                            Timezone = "US Central"
                        }
                    },
                    //Aircraft = t.Aircraft == null ? null : new AircraftDto
                    Aircraft = new AircraftDto
                    {
                        AircraftId = 545345,
                        RegistrationNumber = "#fntrv245",
                        AircraftType = "sbfobdosb",
                        Manufacturer = "Boeing",
                        Model = "757",
                        TotalSeats = 560,
                        EconomySeats = 300,
                        BusinessSeats = 100,
                        FirstClassSeats = 50,
                        CurrentStatus = "Schedule",
                        LastMaintenanceDate = new DateTime(2025, 7, 8, 9, 45, 0),
                        NextMaintenanceDate = new DateTime(2006, 5, 14),
                        IsActive = true
                    },
                    //Recurrence = t.Recurrence == null ? null : new RecurrenceDto
                    Recurrence = new RecurrenceDto
                    {
                        RecurrenceType = "Weekly",
                        DaysOfWeek = new List<string> { "Monday", "Wednesday", "Friday" },
                        DayOfMonth = null
                    },
                    //Pricings = t.Pricings?.Select(p => new FlightPriceDto
                    Pricings = new List<FlightPriceDto>
                    {
                        new FlightPriceDto
                        {
                            ClassType = "Economy",
                            FlightId = 3434,
                            Price = 480
                        }
                    }
                },
                    new FlightScheduleTemplateDto
                    {
                         TemplateId = 123,
                    TemplateName = "Morning Express",
                    FlightNumberPrefix = "AM · B777-300ER",
                    StartDate = DateTime.Parse("Jan 01 2026"),
                    EndDate = DateTime.Parse("Dec 31 2026"),
                    DepartureTime = TimeSpan.Parse("09:45"),
                    ArrivalTime = TimeSpan.Parse("17:30"),
                    IsActive = true,
                    Route = new RouteDto
                    {
                        RouteId = 342,
                        RouteCode = "NYC-LAX",
                        Distance = 2000,
                        EstimatedDuration = 290,
                        //OriginAirport = Route.OriginAirport == null ? null : new AirportDto
                        OriginAirport =  new AirportDto
                        {
                            AirportId = 3456,
                            AirportCode = "NYC",
                            City = "Santadr",
                            AirportName = "tspgptp",
                            State = "slfbobfg",
                            Country = "sbfosbdo",
                            Latitude = 35.754543564m,
                            Longitude = 24.305983684m,
                            Timezone = "US_Central"
                        },
                        //DestinationAirport = t.Route.DestinationAirport == null ? null : new AirportDto
                        DestinationAirport = new AirportDto
                        {
                            AirportId = 2423,
                            AirportCode = "LAX",
                            City = "fdlbf",
                            AirportName = "dfoubos",
                            State = "dlhbsob",
                            Country = "dfobosr",
                            Latitude = 45.983578m,
                            Longitude = 45.2582588m,
                            Timezone = "US Central"
                        }
                    },
                    //Aircraft = t.Aircraft == null ? null : new AircraftDto
                    Aircraft = new AircraftDto
                    {
                        AircraftId = 545345,
                        RegistrationNumber = "#fntrv245",
                        AircraftType = "sbfobdosb",
                        Manufacturer = "Boeing",
                        Model = "757",
                        TotalSeats = 560,
                        EconomySeats = 300,
                        BusinessSeats = 100,
                        FirstClassSeats = 50,
                        CurrentStatus = "Schedule",
                        LastMaintenanceDate = new DateTime(2025, 7, 8, 9, 45, 0),
                        NextMaintenanceDate = new DateTime(2006, 5, 14),
                        IsActive = true
                    },
                    //Recurrence = t.Recurrence == null ? null : new RecurrenceDto
                    Recurrence = new RecurrenceDto
                    {
                        RecurrenceType = "Weekly",
                        DaysOfWeek = new List<string> { "Monday", "Wednesday", "Friday" },
                        DayOfMonth = null
                    },
                    //Pricings = t.Pricings?.Select(p => new FlightPriceDto
                    Pricings = new List<FlightPriceDto>
                    {
                        new FlightPriceDto
                        {
                            ClassType = "Economy",
                            FlightId = 3434,
                            Price = 480
                        }
                    }
                }
                };

                //var result = new PagedResultDto<FlightScheduleTemplateDto>
                //{
                //    PageNumber = request.pageNumber,
                //    PageSize = request.pageSize,
                //    TotalRecords = 345,
                //    Data = dtos
                //};

                //return ApiResponse<IEnumerable<FlightScheduleTemplateDto>>.SuccessResponse(dtos);
                return ApiResponse<IEnumerable<FlightScheduleTemplateDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<FlightScheduleTemplateDto>>.ErrorResponse("Error fetching templates", new[] { ex.Message });
            }
        }
    }
}


//need to generate SP for this
