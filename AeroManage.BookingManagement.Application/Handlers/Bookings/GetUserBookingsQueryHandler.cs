using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Bookings;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Bookings
{
    public class GetUserBookingsQueryHandler : IRequestHandler<GetUserBookingsQuery, ApiResponse<PagedResultDto<BookingDto>>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly ICacheService _cache;
        private readonly ILogger<GetUserBookingsQueryHandler> _logger;

        public GetUserBookingsQueryHandler(
            IBookingRepository bookingRepo,
            ICacheService cache,
            ILogger<GetUserBookingsQueryHandler> logger)
        {
            _bookingRepo = bookingRepo;
            _cache = cache;
            _logger = logger;
        }
        public async Task<ApiResponse<PagedResultDto<BookingDto>>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Try cache first
                var cacheKey = $"user:bookings:{request.userId}:{request.pageNumber}:{request.pageSize}";
                var cached = await _cache.GetAsync<PagedResultDto<BookingDto>>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<PagedResultDto<BookingDto>>.SuccessResponse(cached);
                }

                // Get from repository
                var (bookings, totalRecords) = await _bookingRepo.GetUserBookingsAsync(
                    request.userId,
                    request.pageNumber,
                    request.pageSize
                );

                var dtos = bookings.Select(b => new BookingDto
                {
                    BookingId = b.BookingId,
                    BookingReference = b.BookingReference,
                    PNR = b.PNR,
                    BookingStatus = b.BookingStatus,
                    TotalAmount = b.TotalAmount,
                    Currency = b.Currency,
                    PaymentStatus = b.PaymentStatus,
                    BookingDate = b.BookingDate,
                    ContactEmail = b.BookingEmail,
                    ContactPhone = b.BookingPhone
                }).ToList();

                var result = new PagedResultDto<BookingDto>
                {
                    Data = dtos,
                    PageNumber = request.pageNumber,
                    PageSize = request.pageSize,
                    TotalRecords = totalRecords
                };

                // Cache for 5 minutes
                await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

                return ApiResponse<PagedResultDto<BookingDto>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting bookings for user {request.userId}");
                return ApiResponse<PagedResultDto<BookingDto>>.ErrorResponse("Failed to retrieve bookings", new[] { ex.Message });
            }
        }
    }
}
