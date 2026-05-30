using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task<(int BoardingPassId, string BoardingPassNumber, string QRCode)> GenerateBoardingPassAsync(
           int bookingPassengerId, string gate, DateTime boardingTime);
        Task<PNRDetails> GetPNRDetailsAsync(string pnr);
        Task<int> LogEmailNotificationAsync(int bookingId, string emailType,
            string recipient, string subject, string status);
        Task<bool> UpdateEmailStatusAsync(int notificationId, string status, string errorMessage);
    }
}
