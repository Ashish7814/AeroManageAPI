using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Implementation
{
    public class PromoCodeRepository : IPromoCodeRepository
    {
        private readonly string _connectionString;

        public PromoCodeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<PromoCode> GetPromoCodeByCodeAsync(string code)
        {
            using var connection = CreateConnection();

            var promoCode = await connection.QueryFirstOrDefaultAsync<PromoCode>(
                "sp_GetPromoCodeByCode",
                new { Code = code },
                commandType: CommandType.StoredProcedure
            );

            return promoCode;
        }

        public async Task<PromoCode> ValidatePromoCodeAsync(string code, decimal bookingAmount, CancellationToken cancellationToken = default)
        {
            using var connection = CreateConnection();

            //var promoCode = await connection.QueryFirstOrDefaultAsync<PromoCode>(
            //    "sp_ValidatePromoCode",
            //    new { Code = code, BookingAmount = bookingAmount },
            //    commandType: CommandType.StoredProcedure
            //);

            //return promoCode;
            return await connection.QueryFirstOrDefaultAsync<PromoCode>(
               new CommandDefinition(
                   "sp_ValidatePromoCode",
                   new { Code = code, BookingAmount = bookingAmount },
                   commandType: CommandType.StoredProcedure,
                   cancellationToken: cancellationToken));
        }

        public async Task<bool> IncrementUsageAsync(int promoCodeId,
            IDbConnection connection,
            IDbTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            var result = await connection.ExecuteScalarAsync<int>(
              new CommandDefinition(
                  "sp_IncrementPromoCodeUsage",
                  new { PromoCodeId = promoCodeId },
                  transaction,
                  commandType: CommandType.StoredProcedure,
                  cancellationToken: cancellationToken));

            return result == 1;
        }

        public async Task<IEnumerable<PromoCode>> GetActivePromoCodesAsync()
        {
            using var connection = CreateConnection();

            var promoCodes = await connection.QueryAsync<PromoCode>(
                "sp_GetActivePromoCodes",
                commandType: CommandType.StoredProcedure
            );

            return promoCodes;
        }

        public async Task<int> CreatePromoCodeAsync(PromoCode promoCode)
        {
            using var connection = CreateConnection();

            var promoCodeId = await connection.ExecuteScalarAsync<int>(
                "sp_CreatePromoCode",
                new
                {
                    Code = promoCode.Code,
                    DiscountType = promoCode.DiscountType,
                    DiscountValue = promoCode.DiscountValue,
                    MinimumAmount = promoCode.MinimumAmount,
                    MaximumDiscount = promoCode.MaximumDiscount,
                    UsageLimit = promoCode.UsageLimit,
                    ValidFrom = promoCode.ValidFrom,
                    ValidUntil = promoCode.ValidUntil
                },
                commandType: CommandType.StoredProcedure
            );

            return promoCodeId;
        }

        public async Task<bool> UpdatePromoCodeAsync(PromoCode promoCode)
        {
            using var connection = CreateConnection();

            var result = await connection.ExecuteScalarAsync<int>(
                "sp_UpdatePromoCode",
                new
                {
                    PromoCodeId = promoCode.PromoCodeId,
                    DiscountType = promoCode.DiscountType,
                    DiscountValue = promoCode.DiscountValue,
                    MinimumAmount = promoCode.MinimumAmount,
                    MaximumDiscount = promoCode.MaximumDiscount,
                    UsageLimit = promoCode.UsageLimit,
                    ValidFrom = promoCode.ValidFrom,
                    ValidUntil = promoCode.ValidUntil,
                    IsActive = promoCode.IsActive
                },
                commandType: CommandType.StoredProcedure
            );

            return result == 1;
        }

        public async Task<bool> DeactivatePromoCodeAsync(int promoCodeId)
        {
            using var connection = CreateConnection();

            var result = await connection.ExecuteScalarAsync<int>(
                "sp_DeactivatePromoCode",
                new { PromoCodeId = promoCodeId },
                commandType: CommandType.StoredProcedure
            );

            return result == 1;
        }
    }
}
