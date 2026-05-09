using AeroManage.BookingManagement.Application.Commands.Passengers;
using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Passengers
{
    public class AddPassengerCommandHandler : IRequestHandler<AddPassengerCommand, ApiResponse<PassengerDetailsDto>>
    {
        private readonly IPassengerRepository _passengerRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<AddPassengerCommandHandler> _logger;

        public AddPassengerCommandHandler(
            IPassengerRepository passengerRepo,
            ICacheService cache,
            ILogger<AddPassengerCommandHandler> logger)
        {
            _passengerRepo = passengerRepo;
            _cache = cache;
            _logger = logger;
        }

        //public async Task<ApiResponse<PassengerDetailsDto>> Handle(AddPassengerCommand request, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var passenger = new Passenger
        //        {
        //            FirstName = request.dto.Passenger.FirstName,
        //            LastName = request.dto.Passenger.LastName,
        //            DateOfBirth = request.dto.Passenger.DateOfBirth,
        //            Gender = request.dto.Passenger.Gender,
        //            Nationality = request.dto.Passenger.Nationality,
        //            PassportNumber = request.dto.Passenger.PassportNumber,
        //            PassportExpiry = request.dto.Passenger.PassportExpiry,
        //            Email = request.dto.Passenger.Email,
        //            Phone = request.dto.Passenger.Phone
        //        };

        //        var created = await _passengerRepo.CreatePassengerAsync(passenger);

        //        var bookingPassenger = new BookingPassenger
        //        {
        //            BookingId = request.bookingId,
        //            PassengerId = created.PassengerId,
        //            PassengerType = request.dto.Passenger.PassengerType,
        //            SeatClass = request.dto.Passenger.SeatClass
        //        };

        //        await _passengerRepo.AddPassengerToBookingAsync(bookingPassenger);

        //        // Invalidate booking cache
        //        await _cache.RemoveAsync($"booking:summary:{request.bookingId}");

        //        var result = new PassengerDetailsDto
        //        {
        //            PassengerId = created.PassengerId,
        //            FirstName = created.FirstName,
        //            LastName = created.LastName,
        //            Email = created.Email
        //        };

        //        return ApiResponse<PassengerDetailsDto>.SuccessResponse(result, "Passenger added successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error adding passenger");
        //        return ApiResponse<PassengerDetailsDto>.ErrorResponse("Failed to add passenger", new[] { ex.Message });
        //    }
        //}



            public async Task<ApiResponse<PassengerDetailsDto>> Handle(
                AddPassengerCommand command,
                CancellationToken cancellationToken)
            {
                try
                {
                    var dto = command.dto.Passenger;

                    var passenger = new Passenger
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        DateOfBirth = dto.DateOfBirth,
                        Gender = dto.Gender,
                        Nationality = dto.Nationality,
                        PassportNumber = dto.PassportNumber,
                        PassportExpiry = dto.PassportExpiry,
                        Email = dto.Email,
                        Phone = dto.Phone,
                        CreatedAt = DateTime.UtcNow
                    };

                    using var connection = (SqlConnection)_passengerRepo.CreateConnection();
                    await connection.OpenAsync(cancellationToken);
                    using var transaction = await connection.BeginTransactionAsync(cancellationToken);

                    try
                    {
                    Passenger createdPassenger;
                    var existingPassenger = await _passengerRepo.GetPassengerByIdAsync(dto.PassengerId, dto.Email);
                    if (existingPassenger != null)
                    {
                        createdPassenger = existingPassenger;
                    }
                    else
                    {
                        createdPassenger = await _passengerRepo.CreatePassengerAsync(
                                passenger, connection, transaction, cancellationToken);
                    }
                        

                        var bookingPassenger = new BookingPassenger
                        {
                            BookingId = command.bookingId,
                            PassengerId = createdPassenger.PassengerId,
                            PassengerType = dto.PassengerType,
                            SeatClass = dto.SeatClass,
                            TravelInsurance = dto.TravelInsurance,
                            CreatedAt = DateTime.UtcNow
                        };

                        await _passengerRepo.AddPassengerToBookingAsync(
                            bookingPassenger, connection, transaction, cancellationToken);

                        await transaction.CommitAsync(cancellationToken);

                        // Invalidate booking cache using the shared key helper
                        await _cache.RemoveAsync(BookingCacheKeys.Booking(command.bookingId));

                        _logger.LogInformation(
                            "Passenger added. PassengerId: {PassengerId}, BookingId: {BookingId}",
                            createdPassenger.PassengerId, command.bookingId);

                        var result = new PassengerDetailsDto
                        {
                            PassengerId = createdPassenger.PassengerId,
                            FirstName = createdPassenger.FirstName,
                            LastName = createdPassenger.LastName,
                            Email = createdPassenger.Email
                        };

                        return ApiResponse<PassengerDetailsDto>.SuccessResponse(
                            result, "Passenger added successfully");
                    }
                    catch
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error adding passenger to booking {BookingId}", command.bookingId);

                    return ApiResponse<PassengerDetailsDto>.ErrorResponse(
                        "Failed to add passenger", new[] { ex.Message });
                }
            }


        }
}
