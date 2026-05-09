using AeroManage.BookingManagement.Application.Commands.Bookings;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Hubs;
using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Bookings
{
    /* public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ApiResponse<BookingDto>>
     {
         private readonly IBookingRepository _repo;
         private readonly ICacheService _cache;
         private readonly ILogger<AddBookingAddonsCommandHandler> _logger;

         public CreateBookingCommandHandler(
             IBookingRepository repo,
             ICacheService cache,
             ILogger<AddBookingAddonsCommandHandler> logger)
         {
             _repo = repo ?? throw new ArgumentNullException(nameof(repo));
             _cache = cache;
             _logger = logger;
         }

         public async Task<ApiResponse<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
         {
             try
             {
                 var booking = new Booking { 
                 }
                 var createBooking = await _repo.CreateBookingAsync(request.dto);

             }
         }
     }*/

    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ApiResponse<BookingDto>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IPassengerRepository _passengerRepo;
        private readonly IBookingPricingRepository _pricingRepo;
        private readonly IPromoCodeRepository _promoCodeRepo;
        private readonly ICacheService _cache;
        private readonly IHubContext<BookingHub> _hubContext;
        private readonly ILogger<CreateBookingCommandHandler> _logger;

        public CreateBookingCommandHandler(
            IBookingRepository bookingRepo,
            IPassengerRepository passengerRepo,
            IBookingPricingRepository pricingRepo,
            IPromoCodeRepository promoCodeRepo,
            ICacheService cache,
            IHubContext<BookingHub> hubContext,
            ILogger<CreateBookingCommandHandler> logger)
        {
            _bookingRepo = bookingRepo ?? throw new ArgumentNullException(nameof(bookingRepo));
            _passengerRepo = passengerRepo ?? throw new ArgumentNullException(nameof(passengerRepo));
            _pricingRepo = pricingRepo ?? throw new ArgumentNullException(nameof(pricingRepo));
            _promoCodeRepo = promoCodeRepo ?? throw new ArgumentNullException(nameof(promoCodeRepo));
            _cache = cache;
            _hubContext = hubContext;
            _logger = logger;
        }

        //public async Task<ApiResponse<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        // 1. Generate booking identifiers
        //        var (bookingReference, pnr) = await _bookingRepo.GenerateBookingIdentifiersAsync();

        //        _logger.LogInformation($"Generated booking reference: {bookingReference}, PNR: {pnr}");

        //        // 2. Calculate pricing
        //        var passengers = request.dto.Passengers.Select(p => (
        //            p.PassengerType,
        //            p.SeatClass,
        //            p.bookingAddonsDto.ExtraBaggage,
        //            p.bookingAddonsDto.TravelInsurance
        //        )).ToList();

        //        var totalPrice = await _pricingRepo.CalculateTotalPriceAsync(
        //            request.dto.FlightIds,
        //            passengers,
        //            request.dto.PromoCode
        //        );

        //        // 3. Validate and apply promo code
        //        decimal discountAmount = 0;
        //        int? promoCodeId = null;

        //        if (!string.IsNullOrEmpty(request.dto.PromoCode))
        //        {
        //            var promoCode = await _promoCodeRepo.ValidatePromoCodeAsync(request.dto.PromoCode, totalPrice);

        //            if (promoCode != null && promoCode.ValidationStatus == "Valid")
        //            {
        //                discountAmount = promoCode.CalculatedDiscount;
        //                totalPrice -= discountAmount;
        //                promoCodeId = promoCode.PromoCodeId;

        //                _logger.LogInformation($"Applied promo code {request.dto.PromoCode}: ${discountAmount} discount");
        //            }
        //        }

        //        // 4. Create booking entity
        //        var booking = new Booking
        //        {
        //            BookingReference = bookingReference,
        //            PNR = pnr,
        //            UserId = request.dto.UserId,
        //            TotalAmount = totalPrice,
        //            Currency = "USD",
        //            BookingEmail = request.dto.ContactEmail,
        //            BookingPhone = request.dto.ContactPhone,
        //            SpecialRequests = request.dto.SpecialRequests,
        //            BookingStatus = "Pending",
        //            PaymentStatus = "Pending",
        //            BookingDate = DateTime.UtcNow,
        //            CreatedAt = DateTime.UtcNow
        //        };

        //        // 5. Create booking in database
        //        var createdBooking = await _bookingRepo.CreateBookingAsync(booking);

        //        _logger.LogInformation($"Created booking {createdBooking.BookingId} - Reference: {bookingReference}");

        //        // 6. Add flights to booking
        //        int flightSegment = 1;
        //        foreach (var flightId in request.dto.FlightIds)
        //        {
        //            await _bookingRepo.AddFlightToBookingAsync(
        //                createdBooking.BookingId,
        //                flightId,
        //                flightSegment++
        //            );
        //        }

        //        // 7. Create passengers and add to booking
        //        var passengerIds = new List<int>();

        //        foreach (var passengerDto in request.dto.Passengers)
        //        {
        //            // Create passenger
        //            var passenger = new Passenger
        //            {
        //                UserId = request.dto.UserId,
        //                FirstName = passengerDto.FirstName,
        //                LastName = passengerDto.LastName,
        //                DateOfBirth = passengerDto.DateOfBirth,
        //                Gender = passengerDto.Gender,
        //                Nationality = passengerDto.Nationality,
        //                PassportNumber = passengerDto.PassportNumber,
        //                PassportExpiry = passengerDto.PassportExpiry,
        //                Email = passengerDto.Email,
        //                Phone = passengerDto.Phone,
        //                FrequentFlyerNumber = passengerDto.FrequentFlyerNumber,
        //                CreatedAt = DateTime.UtcNow
        //            };

        //            var createdPassenger = await _passengerRepo.CreatePassengerAsync(passenger);

        //            // Add passenger to booking
        //            var bookingPassenger = new BookingPassenger
        //            {
        //                BookingId = createdBooking.BookingId,
        //                PassengerId = createdPassenger.PassengerId,
        //                PassengerType = passengerDto.PassportNumber,
        //                SeatClass = passengerDto.SeatClass,
        //                MealPreference = passengerDto.mealPreferenceDto.MealType,
        //                SpecialAssistance = passengerDto.specialAssistanceDto.Details,
        //                ExtraBaggage = passengerDto.bookingAddonsDto.ExtraBaggage,
        //                CreatedAt = DateTime.UtcNow
        //            };

        //            await _passengerRepo.AddPassengerToBookingAsync(bookingPassenger);
        //            passengerIds.Add(createdPassenger.PassengerId);
        //        }

        //        // 8. Create pricing record
        //        var pricing = new BookingPricing
        //        {
        //            BookingId = createdBooking.BookingId,
        //            BasePrice = totalPrice + discountAmount,
        //            TaxAmount = 0, // Calculate based on route
        //            ServiceFee = 0,
        //            BaggageFee = 0,
        //            SeatSelectionFee = 0,
        //            InsuranceFee = 0,
        //            DiscountAmount = discountAmount,
        //            PromoCode = request.dto.PromoCode,
        //            TotalAmount = totalPrice,
        //            Currency = "USD",
        //            CreatedAt = DateTime.UtcNow
        //        };

        //        await _pricingRepo.CreatePricingAsync(pricing);

        //        // 9. Increment promo code usage
        //        if (promoCodeId.HasValue)
        //        {
        //            await _promoCodeRepo.IncrementUsageAsync(promoCodeId.Value);
        //        }

        //        // 10. Cache the booking
        //        var cacheKey = $"booking:{createdBooking.BookingId}";
        //        await _cache.SetAsync(cacheKey, createdBooking, TimeSpan.FromMinutes(30));

        //        // 11. SignalR notification
        //        if (request.dto.UserId.HasValue)
        //        {
        //            await _hubContext.Clients.Group($"User_{request.dto.UserId.Value}_Bookings")
        //                .SendAsync("BookingCreated", new
        //                {
        //                    BookingId = createdBooking.BookingId,
        //                    BookingReference = bookingReference,
        //                    PNR = pnr,
        //                    TotalAmount = totalPrice
        //                }, cancellationToken);
        //        }

        //        // 12. Build response DTO
        //        var bookingDto = new BookingDto
        //        {
        //            BookingId = createdBooking.BookingId,
        //            BookingReference = bookingReference,
        //            PNR = pnr,
        //            UserId = request.dto.UserId,
        //            BookingStatus = "Pending",
        //            PaymentStatus = "Pending",
        //            TotalAmount = totalPrice,
        //            Currency = "USD",
        //            BookingDate = createdBooking.BookingDate,
        //            ContactEmail = request.dto.ContactEmail,
        //            ContactPhone = request.dto.ContactPhone,
        //            PassengerCount = request.dto.Passengers.Count,
        //            FlightCount = request.dto.FlightIds.Count,
        //            Flights = new List<BookingFlightDto>(), // Would populate from repository
        //            Passengers = new List<BookingPassengerDto>() // Would populate from repository
        //        };

        //        _logger.LogInformation($"Successfully created booking {createdBooking.BookingId}");

        //        return ApiResponse<BookingDto>.SuccessResponse(
        //            bookingDto,
        //            $"Booking created successfully! Reference: {bookingReference}, PNR: {pnr}"
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating booking");
        //        return ApiResponse<BookingDto>.ErrorResponse(
        //            "Failed to create booking",
        //            new[] { ex.Message }
        //        );
        //    }
        //}



        public async Task<ApiResponse<BookingDto>> Handle(
       CreateBookingCommand request,
       CancellationToken cancellationToken)
        {
            var dto = request.dto;

            try
            {
                // 1. Generate booking identifiers
                var (bookingReference, pnr) =
                    await _bookingRepo.GenerateBookingIdentifiersAsync();

                _logger.LogInformation(
                    "Generated booking identifiers. Reference: {BookingReference}, PNR: {PNR}",
                    bookingReference, pnr);

                // 2. Validate promo code (optional) — no price adjustment on the backend
                int? promoCodeId = null;

                if (!string.IsNullOrWhiteSpace(dto.PromoCode))
                {
                    var promoCode = await _promoCodeRepo.ValidatePromoCodeAsync(
                        dto.PromoCode, dto.TotalAmount, cancellationToken);

                    if (promoCode?.ValidationStatus == "Valid")
                    {
                        promoCodeId = promoCode.PromoCodeId;
                        _logger.LogInformation(
                            "Promo code validated. Code: {PromoCode}, BookingReference: {BookingReference}",
                            dto.PromoCode, bookingReference);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Invalid or expired promo code. Code: {PromoCode}", dto.PromoCode);
                    }
                }

                // 3. Build booking entity
                var booking = new Booking
                {
                    BookingReference = bookingReference,
                    PNR = pnr,
                    UserId = dto.UserId,
                    TotalAmount = dto.TotalAmount,   // Trusted from frontend
                    Currency = "USD",
                    BookingEmail = dto.ContactEmail,
                    BookingPhone = dto.ContactPhone,
                    SpecialRequests = dto.SpecialRequests,
                    BookingStatus = "Pending",
                    PaymentStatus = "Pending",
                    BookingDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                // 4. Persist everything in one transaction — no partial failures
                BookingDto bookingDto;

                using (var connection = (SqlConnection)_bookingRepo.CreateConnection())
                {
                    await connection.OpenAsync(cancellationToken);
                    using var transaction = await connection.BeginTransactionAsync(cancellationToken);

                    try
                    {
                        // 4a. Create booking row
                        var createdBooking = await _bookingRepo.CreateBookingAsync(
                            booking, connection, transaction, cancellationToken);

                        _logger.LogInformation(
                            "Booking row created. BookingId: {BookingId}, Reference: {BookingReference}",
                            createdBooking.BookingId, bookingReference);

                        // 4b. Link flights
                        int segment = 1;
                        foreach (var flightId in dto.FlightIds)
                        {
                            await _bookingRepo.AddFlightToBookingAsync(
                                createdBooking.BookingId, flightId, segment++,
                                connection, transaction, cancellationToken);
                        }

                        // 4c. Create passengers and link to booking
                        foreach (var passengerDto in dto.Passengers)
                        {
                            var passenger = new Passenger
                            {
                                UserId = dto.UserId,
                                FirstName = passengerDto.FirstName,
                                LastName = passengerDto.LastName,
                                DateOfBirth = passengerDto.DateOfBirth,
                                Gender = passengerDto.Gender,
                                Nationality = passengerDto.Nationality,
                                PassportNumber = passengerDto.PassportNumber,
                                PassportExpiry = passengerDto.PassportExpiry,
                                Email = passengerDto.Email,
                                Phone = passengerDto.Phone,
                                FrequentFlyerNumber = passengerDto.FrequentFlyerNumber,
                                CreatedAt = DateTime.UtcNow
                            };

                            Passenger createdPassenger;
                            var existingPassenger = await _passengerRepo.GetPassengerByIdAsync(dto.UserId, dto.ContactEmail);
                            if(existingPassenger != null){
                                createdPassenger = existingPassenger;
                            }
                            else
                            {
                                createdPassenger = await _passengerRepo.CreatePassengerAsync(
                                   passenger, connection, transaction, cancellationToken);
                            }

                            var bookingPassenger = new BookingPassenger
                            {
                                BookingId = createdBooking.BookingId,
                                PassengerId = createdPassenger.PassengerId,
                                PassengerType = passengerDto.PassengerType,     // ← FIXED (was PassportNumber)
                                SeatClass = passengerDto.SeatClass,
                                MealPreference = passengerDto.mealPreferenceDto.MealType,
                                SpecialAssistance = passengerDto.specialAssistanceDto.Details,
                                ExtraBaggage = passengerDto.bookingAddonsDto.ExtraBaggage,
                                TravelInsurance = passengerDto.TravelInsurance,   // ← now persisted
                                CreatedAt = DateTime.UtcNow
                            };

                            await _passengerRepo.AddPassengerToBookingAsync(
                                bookingPassenger, connection, transaction, cancellationToken);
                        }

                        // 4d. Create pricing record
                        var pricing = new BookingPricing
                        {
                            BookingId = createdBooking.BookingId,
                            TotalAmount = dto.TotalAmount,
                            DiscountAmount = 0,             // Discount already applied on frontend
                            PromoCode = dto.PromoCode,
                            Currency = "USD",
                            CreatedAt = DateTime.UtcNow
                        };

                        await _bookingRepo.CreatePricingAsync(
                            pricing, connection, transaction, cancellationToken);

                        // 4e. Increment promo code usage (inside same transaction)
                        if (promoCodeId.HasValue)
                        {
                            await _promoCodeRepo.IncrementUsageAsync(
                                promoCodeId.Value, connection, transaction, cancellationToken);
                        }

                        await transaction.CommitAsync(cancellationToken);

                        _logger.LogInformation(
                            "Booking transaction committed. BookingId: {BookingId}", createdBooking.BookingId);

                        // 5. Fetch full detail to populate response (outside transaction — read-only)
                        var flights = await _bookingRepo.GetBookingFlightsAsync(
                            createdBooking.BookingId, cancellationToken);

                        var passengers = await _passengerRepo.GetBookingPassengersAsync(
                            createdBooking.BookingId, cancellationToken);

                        // 6. Cache the booking
                        await _cache.SetAsync(
                            BookingCacheKeys.Booking(createdBooking.BookingId),
                            createdBooking,
                            TimeSpan.FromMinutes(30));

                        // 7. SignalR notification
                        if (dto.UserId > 0)
                        {
                            await _hubContext.Clients
                                .Group($"User_{dto.UserId}_Bookings")
                                .SendAsync("BookingCreated", new
                                {
                                    BookingId = createdBooking.BookingId,
                                    BookingReference = bookingReference,
                                    PNR = pnr,
                                    TotalAmount = dto.TotalAmount
                                }, cancellationToken);
                        }

                        // 8. Build response DTO with full data
                        bookingDto = new BookingDto
                        {
                            BookingId = createdBooking.BookingId,
                            BookingReference = bookingReference,
                            PNR = pnr,
                            UserId = dto.UserId,
                            BookingStatus = "Pending",
                            PaymentStatus = "Pending",
                            TotalAmount = dto.TotalAmount,
                            Currency = "USD",
                            BookingDate = createdBooking.BookingDate,
                            ContactEmail = dto.ContactEmail,
                            ContactPhone = dto.ContactPhone,
                            PassengerCount = dto.Passengers.Count,
                            FlightCount = dto.FlightIds.Count,
                            Flights = flights.Select(f => new BookingFlightDto
                            {
                                FlightId = f.FlightId,
                                FlightNumber = f.FlightNumber,
                                FlightSegment = f.FlightSegment,
                                DepartureDateTime = f.DepartureDateTime,
                                ArrivalDateTime = f.ArrivalDateTime,
                                OriginCode = f.OriginCode,
                                DestinationCode = f.DestinationCode
                            }).ToList(),
                            Passengers = passengers.Select(p => new BookingPassengerDto
                            {
                                PassengerId = p.PassengerId,
                                FirstName = p.Passenger.FirstName,
                                LastName = p.Passenger.LastName,
                                PassengerType = p.PassengerType,
                                SeatClass = p.SeatClass,
                                MealPreference = p.MealPreference,
                                ExtraBaggage = p.ExtraBaggage,
                                TravelInsurance = p.TravelInsurance,
                                Email = p.Passenger.Email,
                                Phone = p.Passenger.Phone
                            }).ToList(),
                        };
                    }
                    catch
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                }

                return ApiResponse<BookingDto>.SuccessResponse(
                    bookingDto,
                    $"Booking created successfully! Reference: {bookingReference}, PNR: {pnr}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error creating booking for user {UserId}", dto.UserId);

                return ApiResponse<BookingDto>.ErrorResponse(
                    "Failed to create booking",
                    new[] { ex.Message });
            }
        }



    }
}
