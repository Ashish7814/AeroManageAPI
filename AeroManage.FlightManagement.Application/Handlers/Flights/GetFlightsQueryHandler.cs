using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Flights;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Flights
{
    public class GetFlightsQueryHandler : IRequestHandler<GetFlightsQuery, ApiResponse<IEnumerable<FlightDto>>>
    {
        private readonly IFlightRepository _repository;

        public GetFlightsQueryHandler(IFlightRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<IEnumerable<FlightDto>>> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
        {
            try
            {
               // var flights = await _repository.GetFlightsAsync();

                // var dtos = flights.Select(f => new FlightDto
                // {
                //     FlightId = f.FlightId,
                //     FlightNumber = f.FlightNumber,
                //     RouteId = f.RouteId,
                //     AircraftId = f.AircraftId,

                //     DepartureDateTime = f.DepartureDateTime,
                //     ArrivalDateTime = f.ArrivalDateTime,

                //     ActualDepartureDateTime = f.ActualDepartureDateTime,
                //     ActualArrivalDateTime = f.ActualArrivalDateTime,

                //     FlightStatus = f.FlightStatus,

                //     CreatedBy = f.CreatedBy,
                //     CreatedAt = f.CreatedAt,
                //     UpdatedAt = f.UpdatedAt,

                //     // Gate Mapping
                //     DepartureGate = f.GateAssignments?
                //.FirstOrDefault(g => g.GateType == "Departure")?.GateNumber,

                //     ArrivalGate = f.GateAssignments?
                //.FirstOrDefault(g => g.GateType == "Arrival")?.GateNumber,

                //     // Price Mapping
                //     EconomyPrice = f.Prices?
                //.FirstOrDefault(p => p.ClassType == "Economy")?.Price ?? 0,

                //     BusinessPrice = f.Prices?
                //.FirstOrDefault(p => p.ClassType == "Business")?.Price ?? 0,

                //     FirstClassPrice = f.Prices?
                //.FirstOrDefault(p => p.ClassType == "First")?.Price ?? 0,

                //     // Seat Mapping
                //     AvailableEconomySeats = f.SeatAvailability?
                //.FirstOrDefault(s => s.ClassType == "Economy")?.AvailableSeats ?? 0,

                //     AvailableBusinessSeats = f.SeatAvailability?
                //.FirstOrDefault(s => s.ClassType == "Business")?.AvailableSeats ?? 0,

                //     AvailableFirstClassSeats = f.SeatAvailability?
                //.FirstOrDefault(s => s.ClassType == "First")?.AvailableSeats ?? 0,

                //     //TotalRecords = f.TotalRecords,

                //     // Route DTO
                //     Route = f.Route != null ? new RouteDto
                //     {
                //         RouteId = f.Route.RouteId,
                //         RouteCode = f.Route.RouteCode,
                //         OriginAirportId = f.Route.OriginAirportId,
                //         DestinationAirportId = f.Route.DestinationAirportId,
                //         Distance = f.Route.Distance,
                //         EstimatedDuration = f.Route.EstimatedDuration,

                //         OriginAirport = f.Route.OriginAirport != null ? new AirportDto
                //         {
                //             AirportCode = f.Route.OriginAirport.AirportCode,
                //             AirportName = f.Route.OriginAirport.AirportName,
                //             City = f.Route.OriginAirport.City,
                //             State = f.Route.OriginAirport.State,
                //             Country = f.Route.OriginAirport.Country,
                //             Latitude = f.Route.OriginAirport.Latitude,
                //             Longitude = f.Route.OriginAirport.Longitude,
                //             Timezone = f.Route.OriginAirport.Timezone,
                //             IsActive = f.Route.OriginAirport.IsActive
                //         } : null,

                //         DestinationAirport = f.Route.DestinationAirport != null ? new AirportDto
                //         {
                //             AirportCode = f.Route.DestinationAirport.AirportCode,
                //             AirportName = f.Route.DestinationAirport.AirportName,
                //             City = f.Route.DestinationAirport.City,
                //             State = f.Route.DestinationAirport.State,
                //             Country = f.Route.DestinationAirport.Country,
                //             Latitude = f.Route.DestinationAirport.Latitude,
                //             Longitude = f.Route.DestinationAirport.Longitude,
                //             Timezone = f.Route.DestinationAirport.Timezone,
                //             IsActive = f.Route.DestinationAirport.IsActive
                //         } : null,

                //         TotalRecords = f.Route.TotalRecords
                //     } : null,

                //     // Aircraft DTO
                //     Aircraft = f.Aircraft != null ? new AircraftDto
                //     {
                //         AircraftId = f.Aircraft.AircraftId,
                //         RegistrationNumber = f.Aircraft.RegistrationNumber,
                //         AircraftType = f.Aircraft.AircraftType,
                //         Manufacturer = f.Aircraft.Manufacturer,
                //         Model = f.Aircraft.Model,
                //         YearManufactured = f.Aircraft.YearManufactured,

                //         TotalSeats = f.Aircraft.TotalSeats,
                //         EconomySeats = f.Aircraft.EconomySeats,
                //         BusinessSeats = f.Aircraft.BusinessSeats,
                //         FirstClassSeats = f.Aircraft.FirstClassSeats,

                //         CurrentStatus = f.Aircraft.CurrentStatus,
                //         LastMaintenanceDate = f.Aircraft.LastMaintenanceDate,
                //         NextMaintenanceDate = f.Aircraft.NextMaintenanceDate,
                //         IsActive = f.Aircraft.IsActive
                //     } : null
                // }).ToList();

                var dtos = new List<FlightDto>
                {
                    new FlightDto
                    {
                        FlightId = 1,
                        FlightNumber = "AB123",
                        RouteId = 101,
                        AircraftId = 201,

                        DepartureDateTime = new DateTime(2026, 3, 10, 9, 0, 0),
                        ArrivalDateTime = new DateTime(2026, 3, 10, 12, 0, 0),

                        ActualDepartureDateTime = new DateTime(2026, 3, 10, 9, 15, 0),
                        ActualArrivalDateTime = new DateTime(2026, 3, 10, 12, 5, 0),

                        FlightStatus = "OnTime",

                        CreatedBy = "System",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,

                        // Gate Mapping
                        DepartureGate = "D5",
                        ArrivalGate = "A2",

                        // Price Mapping
                        EconomyPrice = 150,
                        BusinessPrice = 450,
                        FirstClassPrice = 900,

                        // Seat Mapping
                        AvailableEconomySeats = 120,
                        AvailableBusinessSeats = 30,
                        AvailableFirstClassSeats = 10,

                        // Route DTO
                        Route = new RouteDto
                        {
                            RouteId = 101,
                            RouteCode = "NYC-LAX",
                            OriginAirportId = 1,
                            DestinationAirportId = 2,
                            Distance = 2475, // in miles
                            EstimatedDuration = 2,

                            OriginAirport = new AirportDto
                            {
                                AirportCode = "JFK",
                                AirportName = "John F. Kennedy International Airport",
                                City = "New York",
                                State = "NY",
                                Country = "USA",
                                Latitude = 40.6413M,
                                Longitude = -73.7781M,
                                Timezone = "Eastern Standard Time",
                                IsActive = true
                            },

                            DestinationAirport = new AirportDto
                            {
                                AirportCode = "LAX",
                                AirportName = "Los Angeles International Airport",
                                City = "Los Angeles",
                                State = "CA",
                                Country = "USA",
                                Latitude = 33.9416M,
                                Longitude = -118.4085M,
                                Timezone = "Pacific Standard Time",
                                IsActive = true
                            },

                            TotalRecords = 1
                        },

                        // Aircraft DTO
                        Aircraft = new AircraftDto
                        {
                            AircraftId = 201,
                            RegistrationNumber = "N123AB",
                            AircraftType = "Boeing 737",
                            Manufacturer = "Boeing",
                            Model = "737-800",
                            YearManufactured = 2015,

                            TotalSeats = 160,
                            EconomySeats = 120,
                            BusinessSeats = 30,
                            FirstClassSeats = 10,

                            CurrentStatus = "Active",
                            LastMaintenanceDate = new DateTime(2026, 2, 1),
                            NextMaintenanceDate = new DateTime(2026, 5, 1),
                            IsActive = true
                        }
                    },
                    new FlightDto
                    {
                        FlightId = 2,
                        FlightNumber = "AM204",
                        RouteId = 102,
                        AircraftId = 202,

                        DepartureDateTime = new DateTime(2026,3,11,9,45,0),
                        ArrivalDateTime = new DateTime(2026,3,11,13,30,0),

                        ActualDepartureDateTime = new DateTime(2026,3,11,9,50,0),
                        ActualArrivalDateTime = null,

                        FlightStatus = "InFlight",

                        CreatedBy = "System",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,

                        DepartureGate = "C3",
                        ArrivalGate = "B6",

                        EconomyPrice = 220,
                        BusinessPrice = 540,
                        FirstClassPrice = 980,

                        AvailableEconomySeats = 110,
                        AvailableBusinessSeats = 28,
                        AvailableFirstClassSeats = 8,

                        Route = new RouteDto
                        {
                            RouteId = 102,
                            RouteCode = "SIN-DXB",
                            OriginAirportId = 3,
                            DestinationAirportId = 4,
                            Distance = 3645,
                            EstimatedDuration = 7,

                            OriginAirport = new AirportDto
                            {
                                AirportCode = "SIN",
                                AirportName = "Singapore Changi Airport",
                                City = "Singapore",
                                Country = "Singapore",
                                Latitude = 1.3644M,
                                Longitude = 103.9915M,
                                Timezone = "Singapore Standard Time",
                                IsActive = true
                            },

                            DestinationAirport = new AirportDto
                            {
                                AirportCode = "DXB",
                                AirportName = "Dubai International Airport",
                                City = "Dubai",
                                Country = "UAE",
                                Latitude = 25.2532M,
                                Longitude = 55.3657M,
                                Timezone = "Gulf Standard Time",
                                IsActive = true
                            }
                        },

                        Aircraft = new AircraftDto
                        {
                            AircraftId = 202,
                            RegistrationNumber = "A6-DFX",
                            AircraftType = "Airbus A320",
                            Manufacturer = "Airbus",
                            Model = "A320-200",
                            YearManufactured = 2018,

                            TotalSeats = 150,
                            EconomySeats = 120,
                            BusinessSeats = 24,
                            FirstClassSeats = 6,

                            CurrentStatus = "Active",
                            LastMaintenanceDate = new DateTime(2026,1,10),
                            NextMaintenanceDate = new DateTime(2026,4,10),
                            IsActive = true
                        }
                    },
                    new FlightDto
                    {
                        FlightId = 3,
                        FlightNumber = "AM105",
                        RouteId = 103,
                        AircraftId = 203,

                        DepartureDateTime = new DateTime(2026,3,12,11,0,0),
                        ArrivalDateTime = new DateTime(2026,3,12,17,0,0),

                        ActualDepartureDateTime = null,
                        ActualArrivalDateTime = null,

                        FlightStatus = "Boarding",

                        CreatedBy = "System",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,

                        DepartureGate = "C4",
                        ArrivalGate = "A3",

                        EconomyPrice = 300,
                        BusinessPrice = 650,
                        FirstClassPrice = 1200,

                        AvailableEconomySeats = 95,
                        AvailableBusinessSeats = 22,
                        AvailableFirstClassSeats = 6,

                        Route = new RouteDto
                        {
                            RouteId = 103,
                            RouteCode = "DXB-LHR",
                            OriginAirportId = 4,
                            DestinationAirportId = 5,
                            Distance = 3400,
                            EstimatedDuration = 6,

                            OriginAirport = new AirportDto
                            {
                                AirportCode = "DXB",
                                AirportName = "Dubai International Airport",
                                City = "Dubai",
                                Country = "UAE",
                                Latitude = 25.2532M,
                                Longitude = 55.3657M,
                                Timezone = "GST",
                                IsActive = true
                            },

                            DestinationAirport = new AirportDto
                            {
                                AirportCode = "LHR",
                                AirportName = "London Heathrow Airport",
                                City = "London",
                                Country = "UK",
                                Latitude = 51.4700M,
                                Longitude = -0.4543M,
                                Timezone = "GMT",
                                IsActive = true
                            }
                        },

                        Aircraft = new AircraftDto
                        {
                            AircraftId = 203,
                            RegistrationNumber = "G-LHRX",
                            AircraftType = "Boeing 777",
                            Manufacturer = "Boeing",
                            Model = "777-300ER",
                            YearManufactured = 2016,

                            TotalSeats = 300,
                            EconomySeats = 220,
                            BusinessSeats = 60,
                            FirstClassSeats = 20,

                            CurrentStatus = "Active",
                            LastMaintenanceDate = new DateTime(2026,2,5),
                            NextMaintenanceDate = new DateTime(2026,5,5),
                            IsActive = true
                        }
                    },
                    new FlightDto
                    {
                        FlightId = 4,
                        FlightNumber = "AM312",
                        RouteId = 104,
                        AircraftId = 204,

                        DepartureDateTime = new DateTime(2026,3,13,14,45,0),
                        ArrivalDateTime = new DateTime(2026,3,14,6,0,0),

                        ActualDepartureDateTime = null,
                        ActualArrivalDateTime = null,

                        FlightStatus = "Delayed",

                        CreatedBy = "System",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,

                        DepartureGate = "D7",
                        ArrivalGate = "E2",

                        EconomyPrice = 450,
                        BusinessPrice = 1100,
                        FirstClassPrice = 2200,

                        AvailableEconomySeats = 85,
                        AvailableBusinessSeats = 18,
                        AvailableFirstClassSeats = 4,

                        Route = new RouteDto
                        {
                            RouteId = 104,
                            RouteCode = "JFK-SIN",
                            OriginAirportId = 1,
                            DestinationAirportId = 3,
                            Distance = 9537,
                            EstimatedDuration = 18,

                            OriginAirport = new AirportDto
                            {
                                AirportCode = "JFK",
                                AirportName = "John F Kennedy Airport",
                                City = "New York",
                                Country = "USA",
                                Latitude = 40.6413M,
                                Longitude = -73.7781M,
                                Timezone = "EST",
                                IsActive = true
                            },

                            DestinationAirport = new AirportDto
                            {
                                AirportCode = "SIN",
                                AirportName = "Singapore Changi Airport",
                                City = "Singapore",
                                Country = "Singapore",
                                Latitude = 1.3644M,
                                Longitude = 103.9915M,
                                Timezone = "SGT",
                                IsActive = true
                            }
                        },

                        Aircraft = new AircraftDto
                        {
                            AircraftId = 204,
                            RegistrationNumber = "N998AB",
                            AircraftType = "Boeing 787",
                            Manufacturer = "Boeing",
                            Model = "787 Dreamliner",
                            YearManufactured = 2020,

                            TotalSeats = 280,
                            EconomySeats = 210,
                            BusinessSeats = 50,
                            FirstClassSeats = 20,

                            CurrentStatus = "Active",
                            LastMaintenanceDate = new DateTime(2026,2,1),
                            NextMaintenanceDate = new DateTime(2026,5,1),
                            IsActive = true
                        }
                    },
                    new FlightDto
                    {
                        FlightId = 5,
                        FlightNumber = "AM088",
                        RouteId = 105,
                        AircraftId = 205,

                        DepartureDateTime = new DateTime(2026,3,14,16,0,0),
                        ArrivalDateTime = new DateTime(2026,3,14,18,30,0),

                        ActualDepartureDateTime = null,
                        ActualArrivalDateTime = null,

                        FlightStatus = "Scheduled",

                        CreatedBy = "System",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,

                        DepartureGate = "F2",
                        ArrivalGate = "D3",

                        EconomyPrice = 120,
                        BusinessPrice = 350,
                        FirstClassPrice = 650,

                        AvailableEconomySeats = 130,
                        AvailableBusinessSeats = 28,
                        AvailableFirstClassSeats = 8,

                        Route = new RouteDto
                        {
                            RouteId = 105,
                            RouteCode = "BOM-DXB",
                            OriginAirportId = 6,
                            DestinationAirportId = 4,
                            Distance = 1920,
                            EstimatedDuration = 3,

                            OriginAirport = new AirportDto
                            {
                                AirportCode = "BOM",
                                AirportName = "Chhatrapati Shivaji Airport",
                                City = "Mumbai",
                                Country = "India",
                                Latitude = 19.0896M,
                                Longitude = 72.8656M,
                                Timezone = "IST",
                                IsActive = true
                            },

                            DestinationAirport = new AirportDto
                            {
                                AirportCode = "DXB",
                                AirportName = "Dubai International Airport",
                                City = "Dubai",
                                Country = "UAE",
                                Latitude = 25.2532M,
                                Longitude = 55.3657M,
                                Timezone = "GST",
                                IsActive = true
                            }
                        },

                        Aircraft = new AircraftDto
                        {
                            AircraftId = 205,
                            RegistrationNumber = "VT-BOM1",
                            AircraftType = "Airbus A321",
                            Manufacturer = "Airbus",
                            Model = "A321neo",
                            YearManufactured = 2019,

                            TotalSeats = 180,
                            EconomySeats = 150,
                            BusinessSeats = 24,
                            FirstClassSeats = 6,

                            CurrentStatus = "Active",
                            LastMaintenanceDate = new DateTime(2026,1,25),
                            NextMaintenanceDate = new DateTime(2026,4,25),
                            IsActive = true
                        }
                    }

                    // Add more FlightDto objects here if needed
                };
                return ApiResponse<IEnumerable<FlightDto>>.SuccessResponse(dtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<FlightDto>>.ErrorResponse("Error fetching flights", new[] { ex.Message });
            }
        }
    }
}
