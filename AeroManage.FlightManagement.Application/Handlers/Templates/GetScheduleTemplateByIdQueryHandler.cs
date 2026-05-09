using AeroManage.FlightManagement.Application.DTOs;
using AeroManage.FlightManagement.Application.Mappers;
using AeroManage.FlightManagement.Application.Queries.FlightSchedule;
using AeroManage.FlightManagement.Application.Services.Implementation;
using AeroManage.FlightManagement.Application.Services.Interfaces;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Implementation;
using AeroManage.Shared.Service.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Application.Handlers.Templates
{
    public class GetScheduleTemplateByIdQueryHandler : IRequestHandler<GetScheduleTemplateByIdQuery, ApiResponse<FlightScheduleTemplateDto>>
    {
        private readonly IFlightScheduleTemplateRepository _repository;
        private readonly ICacheService _cache;

        public GetScheduleTemplateByIdQueryHandler(IFlightScheduleTemplateRepository repository, ICacheService cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<ApiResponse<FlightScheduleTemplateDto>> Handle(GetScheduleTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = CacheKeys.ScheduleTemplate(request.templateId);
                var cached = await _cache.GetAsync<FlightScheduleTemplateDto>(cacheKey);

                if (cached != null)
                {
                    return ApiResponse<FlightScheduleTemplateDto>.SuccessResponse(cached);
                }

                var template = await _repository.GetTemplateByIdAsync(request.templateId);

                if (template == null)
                {
                    return ApiResponse<FlightScheduleTemplateDto>.ErrorResponse("Template not found");
                }

                var dto = Mapper.MapToTemplateDto(template);

                await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(15));

                return ApiResponse<FlightScheduleTemplateDto>.SuccessResponse(dto);
            }
            catch (Exception ex)
            {
                return ApiResponse<FlightScheduleTemplateDto>.ErrorResponse("Error fetching template", new[] { ex.Message });
            }
        }

    }
}
