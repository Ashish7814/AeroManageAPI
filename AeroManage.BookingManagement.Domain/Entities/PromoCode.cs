using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class PromoCode
    {
        public int PromoCodeId { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal? MinimumAmount { get; set; }
        public decimal? MaximumDiscount { get; set; }
        public int? UsageLimit { get; set; }
        public int UsageCount { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Computed properties from validation
        public string ValidationStatus { get; set; }
        public decimal CalculatedDiscount { get; set; }
    }
}
