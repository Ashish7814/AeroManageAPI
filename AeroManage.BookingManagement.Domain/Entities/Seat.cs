using AeroManage.BookingManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class Seat
    {
        public int SeatId { get; set; }
        public int AircraftId { get; set; }
        public string SeatCode { get; set; }
        public int RowNumber { get; set; }      // 1–12
        public string Column { get; set; }
        public bool IsOccupied { get; set; }
        public bool IsAvailable => !IsOccupied;
        // Optional UI modifiers (not explicitly in UI but often needed)
        public bool IsSelected { get; set; }           // frontend state sync
        public bool IsAisle { get; set; }              // based on column gap logic
        public bool IsExitRow { get; set; }
        public bool ExtraLegroom { get; set; }
        public decimal Price { get; set; }
        public SeatAvailability availabilityDto { get; set; }
    }
}
