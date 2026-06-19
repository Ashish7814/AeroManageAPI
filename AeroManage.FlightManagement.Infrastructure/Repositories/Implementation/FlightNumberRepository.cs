
using AeroManage.FlightManagement.Domain.Interfaces;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.DTos;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.FlightManagement.Infrastructure.Repositories.Implementation
{
    public class FlightNumberRepository : IFlightNumberRepository
    {
        private readonly IDapperUnitOfWork _unitOfWork;

        public FlightNumberRepository(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FlightNumberResultDto> GenerateFlightNumberAsync(string prefix)
        {
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<FlightNumberResultDto>(
                "sp_GenerateFlightNumber",
                new { Prefix = prefix },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
