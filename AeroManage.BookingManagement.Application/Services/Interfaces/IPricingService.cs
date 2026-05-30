using AeroManage.BookingManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Services.Interfaces
{
    public interface IPricingService
    {
        Task<PricingResult> CalculateAsync(List<int> flightIds, List<PassengerDetailsDto> passengers, string promoCode);
    }
}
