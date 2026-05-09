using AeroManage.BookingManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Services.Interfaces
{
    public interface IPdfService
    {
        Task<string> GenerateETicketAsync(ETicketDto eTicket);
        Task<string> GenerateInvoiceAsync(InvoiceDto invoice);
        Task<string> GenerateBoardingPassAsync(PassengerTicketDto passenger, FlightDetailsDto flight);
    }
}
