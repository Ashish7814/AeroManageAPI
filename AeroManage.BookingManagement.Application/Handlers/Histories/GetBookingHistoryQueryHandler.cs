using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Histories;
using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Histories
{
    public class GetBookingHistoryQueryHandler : IRequestHandler<GetBookingHistoryQuery, (List<BookingHistoryDto> Bookings, int TotalCount)>
    {
        private readonly IBookingPricingRepository _repository;
        private readonly ILogger<GetBookingHistoryQueryHandler> _logger;

        public GetBookingHistoryQueryHandler(
            IBookingPricingRepository repository,
            ILogger<GetBookingHistoryQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<(List<BookingHistoryDto> Bookings, int TotalCount)> Handle(GetBookingHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving booking history - Page {Page}, Size {PageSize}",
                    request.dto.Page,
                    request.dto.PageSize);

                /* var filter = new BookingHistoryFilter
                 {
                     UserId = request.dto.UserId,
                     BookingReference = request.dto.BookingReference,
                     PNR = request.dto.PNR,
                     Status = request.dto.Status,
                     FromDate = request.dto.FromDate,
                     ToDate = request.dto.ToDate,
                     Page = request.dto.Page,
                     PageSize = request.dto.PageSize
                 };

                 // Get from repository
                 var result = await _repository.GetBookingHistoryAsync(filter);

                 // Map Domain → DTO
                 var bookingDtos = result.Bookings.Select(b => new BookingHistoryDto
                 {
                     BookingId = b.BookingId,
                     BookingReference = b.BookingReference,
                     PNR = b.PNR,
                     BookingStatus = b.BookingStatus,
                     BookingDate = b.BookingDate,
                     TravelDate = b.TravelDate,
                     PassengerCount = b.PassengerCount,
                     FlightCount = b.FlightCount,
                     TotalAmount = b.TotalAmount,
                     Currency = b.Currency,
                     PaymentStatus = b.PaymentStatus,
                     LastModified = b.LastModified,
                     Flights = b.Flights.Select(f => new FlightSummaryDto
                     {
                         FlightId = f.FlightId,
                         FlightNumber = f.FlightNumber,
                         DepartureDateTime = f.DepartureDateTime,
                         ArrivalDateTime = f.ArrivalDateTime,
                         OriginCode = f.OriginCode,
                         OriginCity = f.OriginCity ?? string.Empty,
                         DestinationCode = f.DestinationCode,
                         DestinationCity = f.DestinationCity ?? string.Empty,
                         DestinationCountry = f.DestinationCountry ?? string.Empty,
                         Status = f.FlightStatus
                     }).ToList(),
                     Passengers = b.Passengers.Select(p => new PassengerSummaryDto
                     {
                         FirstName = p.FirstName,
                         LastName = p.LastName,
                         PassengerType = p.PassengerType,
                         SeatNumber = p.SeatNumber,
                         TicketNumber = p.TicketNumber
                     }).ToList()
                 }).ToList();

                 return (bookingDtos, result.TotalCount);*/


                var filter = new BookingHistoryFilter
                {
                    UserId = request.dto.UserId,
                    BookingReference = request.dto.BookingReference,
                    PNR = request.dto.PNR,
                    Status = request.dto.Status,
                    FromDate = request.dto.FromDate,
                    ToDate = request.dto.ToDate,
                    Page = request.dto.Page,
                    PageSize = request.dto.PageSize
                };

                // ----------------- Get from Repository -----------------
                var result = await _repository.GetBookingHistoryAsync(filter);

                // ----------------- Map Domain → DTO -----------------
                var bookingDtos = result.Bookings.Select(b => new BookingHistoryDto
                {
                    BookingId = b.BookingId,
                    BookingReference = b.BookingReference ?? string.Empty,
                    PNR = b.PNR ?? string.Empty,
                    BookingStatus = b.BookingStatus ?? string.Empty,
                    BookingChannel = b.BookingChannel ?? string.Empty,
                    BookingSource = b.BookingSource ?? string.Empty,
                    BookingDate = b.BookingDate,
                    TravelDate = b.TravelDate,
                    PassengerCount = b.PassengerCount,
                    FlightCount = b.FlightCount,
                    TotalAmount = b.TotalAmount,
                    Currency = b.Currency ?? "USD",

                    // ---------------- Fare Details ----------------
                    FareDetails = b.FareDetails == null ? null : new FareDetailsDto
                    {
                        BaseFare = b.FareDetails.BaseFare,
                        Taxes = b.FareDetails.Taxes,
                        Fees = b.FareDetails.Fees,
                        Discount = b.FareDetails.Discount,
                        GrandTotal = b.FareDetails.GrandTotal
                    },

                    // ---------------- Payment ----------------
                    Payment = b.Payment == null ? null : new PaymentStatusDto
                    {
                        PaymentStatus = b.Payment.PaymentStatus ?? string.Empty,
                        Amount = b.Payment.Amount,
                        Currency = b.Payment.Currency ?? "USD",
                        PaymentMethod = b.Payment.PaymentMethod ?? string.Empty,
                        PaymentDate = b.Payment.PaymentDate,
                        TransactionId = b.Payment.TransactionId ?? string.Empty
                    },

                    // ---------------- Audit Info ----------------
                    AuditInfo = b.AuditInfo == null ? null : new AuditInfoDto
                    {
                        CreatedBy = b.AuditInfo.CreatedBy ?? string.Empty,
                        CreatedAt = b.AuditInfo.CreatedAt,
                        LastModifiedBy = b.AuditInfo.LastModifiedBy,
                        LastModifiedAt = b.AuditInfo.LastModifiedAt
                    },

                    // ---------------- Flights ----------------
                    Flights = b.Flights.Select(f => new FlightSummaryDto
                    {
                        FlightId = f.FlightId,
                        FlightNumber = f.FlightNumber ?? string.Empty,
                        AirlineCode = f.AirlineCode ?? string.Empty,
                        AirlineName = f.AirlineName ?? string.Empty,
                        AircraftType = f.AircraftType ?? string.Empty,
                        CabinClass = f.CabinClass ?? string.Empty,
                        DurationMinutes = f.DurationMinutes,
                        Status = f.Status ?? string.Empty,
                        BaggageAllowance = f.BaggageAllowance ?? string.Empty,

                        Departure = new AirportInfoDto
                        {
                            AirportCode = f.Departure?.AirportCode ?? string.Empty,
                            AirportName = f.Departure?.AirportName ?? string.Empty,
                            City = f.Departure?.City ?? string.Empty,
                            Country = f.Departure?.Country ?? string.Empty,
                            DateTime = f.Departure?.DateTime ?? DateTime.MinValue,
                            Terminal = f.Departure?.Terminal ?? string.Empty,
                            Gate = f.Departure?.Gate ?? string.Empty
                        },

                        Arrival = new AirportInfoDto
                        {
                            AirportCode = f.Arrival?.AirportCode ?? string.Empty,
                            AirportName = f.Arrival?.AirportName ?? string.Empty,
                            City = f.Arrival?.City ?? string.Empty,
                            Country = f.Arrival?.Country ?? string.Empty,
                            DateTime = f.Arrival?.DateTime ?? DateTime.MinValue,
                            Terminal = f.Arrival?.Terminal ?? string.Empty,
                            Gate = f.Arrival?.Gate ?? string.Empty
                        }
                    }).ToList(),

                    // ---------------- Passengers ----------------
                    Passengers = b.Passengers.Select(p => new PassengerSummaryDto
                    {
                        PassengerId = p.BookingPassengerId,
                        FirstName = p.FirstName ?? string.Empty,
                        LastName = p.LastName ?? string.Empty,
                        PassengerType = p.PassengerType ?? string.Empty,
                        SeatNumber = p.SeatNumber ?? string.Empty,
                        TicketNumber = p.TicketNumber ?? string.Empty,
                        FrequentFlyerNumber = p.FrequentFlyerNumber,
                        MealPreference = p.MealPreference
                    }).ToList(),

                    // ---------------- Contact Info ----------------
                    ContactInfo = b.ContactInfo == null ? null : new PassengerDetailsDto
                    {
                        PassengerId = b.ContactInfo.PassengerId,
                        FirstName = b.ContactInfo.FirstName ?? string.Empty,
                        LastName = b.ContactInfo.LastName ?? string.Empty,
                        DateOfBirth = b.ContactInfo.DateOfBirth,
                        Gender = b.ContactInfo.Gender ?? string.Empty,
                        Nationality = b.ContactInfo.Nationality ?? string.Empty,
                        PassengerType = b.ContactInfo.PassengerType ?? string.Empty,
                        SeatClass = b.ContactInfo.SeatClass ?? string.Empty,
                        PassportNumber = b.ContactInfo.PassportNumber,
                        PassportExpiry = b.ContactInfo.PassportExpiry,
                        Email = b.ContactInfo.Email,
                        Phone = b.ContactInfo.Phone,
                        FrequentFlyerNumber = b.ContactInfo.FrequentFlyerNumber,
                        bookingAddonsDto = b.ContactInfo.bookingAddons != null
                        ? new BookingAddonsDto
                        {
                            PassengerId = b.ContactInfo.bookingAddons.PassengerId,
                            ExtraBaggage = b.ContactInfo.bookingAddons.ExtraBaggage,
                            TravelInsurance = b.ContactInfo.bookingAddons.TravelInsurance,
                            PriorityBoarding = b.ContactInfo.bookingAddons.PriorityBoarding,
                            LoungeAccess = b.ContactInfo.bookingAddons.LoungeAccess,
                            OtherServices = b.ContactInfo.bookingAddons.OtherServices
                        }
                        : null,
                        mealPreferenceDto = b.ContactInfo.mealPreference != null
                        ? new MealPreferenceDto
                        {
                            BookingPassengerId = b.ContactInfo.mealPreference.BookingPassengerId,
                            MealPreferenceId = b.ContactInfo.mealPreference.MealPreferenceId,
                            MealType = b.ContactInfo.mealPreference.MealType,
                            SpecialInstructions = b.ContactInfo.mealPreference.SpecialInstructions,
                        } : null,
                        specialAssistanceDto = b.ContactInfo.specialAssistance != null
                        ? new SpecialAssistanceDto
                        {
                            PassengerId = b.ContactInfo.specialAssistance.BookingPassengerId,
                            AssistanceId = b.ContactInfo.specialAssistance.AssistanceId,
                            AssistanceType = b.ContactInfo.specialAssistance.AssistanceType,
                            Details = b.ContactInfo.specialAssistance.Details
                        } : null,
                    }

                }).ToList();

                // ---------------- Pagination ----------------
                var pagedBookings = bookingDtos
                    .Skip((request.dto.Page - 1) * request.dto.PageSize)
                    .Take(request.dto.PageSize)
                    .ToList();

                return (pagedBookings, result.TotalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking history");
                throw;
            }
        }
    }
}
