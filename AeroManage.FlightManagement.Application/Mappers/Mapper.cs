using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Mappers
{
    public class Mapper
    {
        public static FlightDto MapToDto(Flight flight)
        {
            return new FlightDto
            {
                FlightId = flight.FlightId,
                FlightNumber = flight.FlightNumber,
                RouteId = flight.RouteId,
                AircraftId = flight.AircraftId,
                DepartureDateTime = flight.DepartureDateTime,
                ArrivalDateTime = flight.ArrivalDateTime,
                ActualDepartureDateTime = flight.ActualDepartureDateTime,
                ActualArrivalDateTime = flight.ActualArrivalDateTime,
                //FlightStatus = flight.FlightStatus,
                //DepartureGate = flight.DepartureGate,
                //ArrivalGate = flight.ArrivalGate,
                //EconomyPrice = flight.EconomyPrice,
                //BusinessPrice = flight.BusinessPrice,
                //FirstClassPrice = flight.FirstClassPrice,
                //AvailableEconomySeats = flight.AvailableEconomySeats,
                //AvailableBusinessSeats = flight.AvailableBusinessSeats,
                //AvailableFirstClassSeats = flight.AvailableFirstClassSeats,
                //RouteCode = flight.RouteCode,
                //Distance = flight.Distance,
                //EstimatedDuration = flight.EstimatedDuration,
                //OriginCode = flight.OriginCode,
                //OriginName = flight.OriginName,
                //OriginCity = flight.OriginCity,
                //OriginCountry = flight.OriginCountry,
                //DestinationCode = flight.DestinationCode,
                //DestinationName = flight.DestinationName,
                //DestinationCity = flight.DestinationCity,
                //DestinationCountry = flight.DestinationCountry,
                //RegistrationNumber = flight.RegistrationNumber,
                //AircraftType = flight.AircraftType,
                //TotalSeats = flight.TotalSeats
            };
        }

        public static FlightScheduleTemplateDto MapToTemplateDto(FlightScheduleTemplate t)
        {
            return new FlightScheduleTemplateDto
            {
                TemplateId = t.Id,
                TemplateName = t.TemplateName,
                FlightNumberPrefix = t.FlightNumberPrefix,
                //RouteId = t.RouteId,
                //AircraftId = t.AircraftId,
                //RecurrenceType = t.RecurrenceType,
                //DaysOfWeek = t.DaysOfWeek,
                //DayOfMonth = t.DayOfMonth,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                DepartureTime = t.DepartureTime,
                ArrivalTime = t.ArrivalTime,
                //EconomyPrice = t.EconomyPrice,
                //BusinessPrice = t.BusinessPrice,
                //FirstClassPrice = t.FirstClassPrice,
                IsActive = t.IsActive
            };
        }

    }
}
