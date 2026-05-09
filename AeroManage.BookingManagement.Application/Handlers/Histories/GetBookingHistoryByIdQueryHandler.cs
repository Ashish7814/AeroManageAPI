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
    public class GetBookingHistoryByIdQueryHandler : IRequestHandler<GetBookingHistoryByIdQuery, (List<BookingHistoryDto> Bookings, int TotalCount)>
    {
        private readonly IBookingPricingRepository _repository;
        private readonly ILogger<GetBookingHistoryQueryHandler> _logger;

        public GetBookingHistoryByIdQueryHandler(
            IBookingPricingRepository repository,
            ILogger<GetBookingHistoryQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<(List<BookingHistoryDto> Bookings, int TotalCount)> Handle(GetBookingHistoryByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving booking history - Page {Page}, Size {PageSize}",
                    request.dto.Page,
                    request.dto.PageSize);

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
                    Flights = b.Flights.Select(f => new FlightSummaryDto
                    {
                        FlightId = f.FlightId,
                        FlightNumber = f.FlightNumber,
                        DepartureDateTime = f.DepartureDateTime,
                        ArrivalDateTime = f.ArrivalDateTime,
                        Status = f.Status
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

                return (bookingDtos, result.TotalCount);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking history");
                throw;
            }
        }
    }
}
