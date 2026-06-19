using AeroManage.BookingManagement.Domain.Entities;
using AeroManage.BookingManagement.Domain.Interfaces;
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
        private readonly IDapperUnitOfWork _unitOfWork;
        public PromoCodeRepository(IDapperUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<PromoCode> GetPromoCodeByCodeAsync(string code)
        {
            var promoCode = await _unitOfWork.Connection.QueryFirstOrDefaultAsync<PromoCode>(
                "sp_GetPromoCodeByCode",
                new { Code = code },
                commandType: CommandType.StoredProcedure
            );

            return promoCode;
        }

        public async Task<PromoCode> ValidatePromoCodeAsync(string code, decimal bookingAmount, CancellationToken cancellationToken = default)
        {
            //var promoCode = await connection.QueryFirstOrDefaultAsync<PromoCode>(
            //    "sp_ValidatePromoCode",
            //    new { Code = code, BookingAmount = bookingAmount },
            //    commandType: CommandType.StoredProcedure
            //);

            //return promoCode;
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<PromoCode>(
               new CommandDefinition(
                   "sp_ValidatePromoCode",
                   new { Code = code, BookingAmount = bookingAmount },
                   commandType: CommandType.StoredProcedure,
                   cancellationToken: cancellationToken));
        }

        public async Task<bool> IncrementUsageAsync(int promoCodeId, CancellationToken cancellationToken = default)
        {
            var result = await _unitOfWork.Connection.ExecuteScalarAsync<int>(
              new CommandDefinition(
                  "sp_IncrementPromoCodeUsage",
                  new { PromoCodeId = promoCodeId },
                  commandType: CommandType.StoredProcedure,
                  cancellationToken: cancellationToken));

            return result == 1;
        }

        public async Task<IEnumerable<PromoCode>> GetActivePromoCodesAsync()
        {
            var promoCodes = await _unitOfWork.Connection.QueryAsync<PromoCode>(
                "sp_GetActivePromoCodes",
                commandType: CommandType.StoredProcedure
            );

            return promoCodes;
        }

        public async Task<int> CreatePromoCodeAsync(PromoCode promoCode)
        {
            var promoCodeId = await _unitOfWork.Connection.ExecuteScalarAsync<int>(
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
            var result = await _unitOfWork.Connection.ExecuteScalarAsync<int>(
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
            var result = await _unitOfWork.Connection.ExecuteScalarAsync<int>(
                "sp_DeactivatePromoCode",
                new { PromoCodeId = promoCodeId },
                commandType: CommandType.StoredProcedure
            );

            return result == 1;
        }
    }
}
