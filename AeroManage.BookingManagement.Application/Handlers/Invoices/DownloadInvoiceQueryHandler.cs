using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Application.Queries.Invoices;
using AeroManage.BookingManagement.Application.Services.Interfaces;
using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Handlers.Invoices
{
    public class DownloadInvoiceQueryHandler : IRequestHandler<DownloadInvoiceQuery, ApiResponse<byte[]>>
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IPdfService _pdfService;
        private readonly ILogger<DownloadInvoiceQueryHandler> _logger;

        public DownloadInvoiceQueryHandler(
            IBookingRepository bookingRepo,
            IPdfService pdfService,
            ILogger<DownloadInvoiceQueryHandler> logger)
        {
            _bookingRepo = bookingRepo;
            _pdfService = pdfService;
            _logger = logger;
        }

        public async Task<ApiResponse<byte[]>> Handle(DownloadInvoiceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var booking = await _bookingRepo.GetBookingByIdAsync(request.bookingId);

                if (booking == null)
                {
                    return ApiResponse<byte[]>.ErrorResponse("Booking not found");
                }

                var invoice = new InvoiceDto
                {
                    InvoiceNumber = $"INV{booking.BookingId:D8}",
                    InvoiceDate = DateTime.UtcNow,
                    BookingReference = booking.BookingReference,
                    Pricing = new BookingPricingDto
                    {
                        TotalAmount = booking.TotalAmount,
                        Currency = booking.Currency
                    }
                };

                var pdfPath = await _pdfService.GenerateInvoiceAsync(invoice);
                var pdfBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);

                return ApiResponse<byte[]>.SuccessResponse(pdfBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating invoice for booking {request.bookingId}");
                return ApiResponse<byte[]>.ErrorResponse("Failed to generate invoice", new[] { ex.Message });
            }
        }
    }
}
