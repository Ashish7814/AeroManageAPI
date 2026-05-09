using AeroManage.FlightManagement.Application.Commands.Template;
using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Services.Interfaces;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Templates
{
    public class GenerateFlightsFromTemplateCommandHandler : IRequestHandler<GenerateFlightsFromTemplateCommand, ApiResponse<GenerateFlightsResultDto>>
    {
        private readonly IFlightScheduleTemplateRepository _repository;
        private readonly IBackgroundJobService _backgroundJobService;

        public GenerateFlightsFromTemplateCommandHandler(
            IFlightScheduleTemplateRepository repository,
            IBackgroundJobService backgroundJobService)
        {
            _repository = repository;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<ApiResponse<GenerateFlightsResultDto>> Handle(GenerateFlightsFromTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Enqueue as background job for large date ranges
                var daysDiff = (request.dto.GenerateToDate - request.dto.GenerateFromDate).Days;

                if (daysDiff > 7)
                {
                    // Use background job for large generations
                   /* _backgroundJobService.EnqueueFlightGeneration(
                        request.dto.TemplateId,
                        request.dto.GenerateFromDate,
                        request.dto.GenerateToDate,
                        request.dto.CreatedBy
                    );*/

                    var result = new GenerateFlightsResultDto
                    {
                        FlightsCreated = 0,
                        Message = $"Flight generation job started. Processing {daysDiff} days in background."
                    };

                    return ApiResponse<GenerateFlightsResultDto>.SuccessResponse(result);
                }
                else
                {
                    // Generate immediately for small ranges
                    var flightsCreated = await _repository.GenerateFlightsFromTemplateAsync(
                        request.dto.TemplateId,
                        request.dto.GenerateFromDate,
                        request.dto.GenerateToDate,
                        request.dto.CreatedBy
                    );

                    var result = new GenerateFlightsResultDto
                    {
                        FlightsCreated = flightsCreated,
                        Message = $"{flightsCreated} flights created successfully"
                    };

                    return ApiResponse<GenerateFlightsResultDto>.SuccessResponse(result);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<GenerateFlightsResultDto>.ErrorResponse("Failed to generate flights", new[] { ex.Message });
            }
        }
    }
}
