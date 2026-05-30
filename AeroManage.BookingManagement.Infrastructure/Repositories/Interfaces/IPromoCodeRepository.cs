using AeroManage.BookingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces
{
    public interface IPromoCodeRepository
    {
        Task<PromoCode> GetPromoCodeByCodeAsync(string code);
        Task<PromoCode> ValidatePromoCodeAsync(string code, decimal bookingAmount, CancellationToken cancellationToken);
        Task<bool> IncrementUsageAsync(int promoCodeId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken);
        Task<IEnumerable<PromoCode>> GetActivePromoCodesAsync();
        Task<int> CreatePromoCodeAsync(PromoCode promoCode);
        Task<bool> UpdatePromoCodeAsync(PromoCode promoCode);
        Task<bool> DeactivatePromoCodeAsync(int promoCodeId);
    }
}
