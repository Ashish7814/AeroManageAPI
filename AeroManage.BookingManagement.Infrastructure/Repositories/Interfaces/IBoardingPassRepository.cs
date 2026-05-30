using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces
{
    public interface IBoardingPassRepository
    {
        Task<(int BoardingPassId, string BoardingPassNumber, string QRCode, string BoardingGroup, int BoardingZone)>
            GenerateBoardingPassAsync(int bookingPassengerId, string gate, DateTime boardingTime, string boardingGroup = null, int? boardingZone = null);

        Task<BoardingPass> GetBoardingPassAsync(int? boardingPassId, string boardingPassNumber, int? bookingPassengerId);

        Task<BoardingPassScan> ScanBoardingPassAsync(string boardingPassNumber, string scannedBy);

        Task<List<BoardingPass>> GetFlightBoardingPassesAsync(int flightId);

        Task<int> UpdateBoardingPassGateAsync(int flightId, string newGate, DateTime? newBoardingTime);
    }
}
