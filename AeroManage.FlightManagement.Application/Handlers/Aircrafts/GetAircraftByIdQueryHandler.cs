using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Queries.Aircraft;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Aircrafts
{
    public class GetAircraftByIdQueryHandler : IRequestHandler<GetAircraftByIdQuery, ApiResponse<AircraftDto>>
    {
        private readonly IAircraftRepository _repository;

        public GetAircraftByIdQueryHandler(IAircraftRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<AircraftDto>> Handle(GetAircraftByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var aircraft = await _repository.GetAircraftByIdAsync(request.aircraftId);

                if (aircraft == null)
                {
                    return ApiResponse<AircraftDto>.ErrorResponse("Aircraft not found");
                }

                var dto = new AircraftDto
                {
                    AircraftId = aircraft.AircraftId,
                    RegistrationNumber = aircraft.RegistrationNumber,
                    AircraftType = aircraft.AircraftType,
                    Manufacturer = aircraft.Manufacturer,
                    Model = aircraft.Model,
                    YearManufactured = aircraft.YearManufactured,
                    TotalSeats = aircraft.TotalSeats,
                    EconomySeats = aircraft.EconomySeats,
                    BusinessSeats = aircraft.BusinessSeats,
                    FirstClassSeats = aircraft.FirstClassSeats,
                    //CurrentStatus = aircraft.CurrentStatus,
                    LastMaintenanceDate = aircraft.LastMaintenanceDate,
                    NextMaintenanceDate = aircraft.NextMaintenanceDate
                };

                return ApiResponse<AircraftDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                return ApiResponse<AircraftDto>.ErrorResponse("Error fetching aircraft", new[] { ex.Message });
            }
        }
    }

}
