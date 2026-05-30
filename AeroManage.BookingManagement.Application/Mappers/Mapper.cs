using AeroManage.BookingManagement.Application.DTOs;
using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Domain.Enums;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Mappers
{
    public class Mapper
    {
        public static SeatDto MapToDto(Seat seat)
        {
            return new SeatDto
            {
                SeatId = seat.SeatId,
                AircraftId = seat.AircraftId,
                SeatCode = seat.SeatCode,
                RowNumber = seat.RowNumber,
                Column = seat.Column,
                IsOccupied = seat.IsOccupied,
                //IsAvailable = seat.IsAvailable,
                IsAisle = seat.IsAisle,
                IsExitRow = seat.IsExitRow,
                ExtraLegroom = seat.ExtraLegroom,
                Price = seat.Price,
                IsSelected = seat.IsSelected,
                availabilityDto = new SeatAvailabilityDto
                {
                    seatClass = seat.availabilityDto.SeatClass,
                    AvailableSeats = seat.availabilityDto.AvailableSeats,
                    BusinessAvailable = seat.availabilityDto.BusinessAvailable,
                    FirstClassAvailable = seat.availabilityDto.FirstClassAvailable,
                    EconomyAvailable = seat.availabilityDto.EconomyAvailable,
                    TotalAvailable = seat.availabilityDto.TotalSeats
                }
            };
        }

    }
}
