using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class PricingSummary
    {
        public decimal BasePrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal BaggageFee { get; set; }
        public decimal SeatSelectionFee { get; set; }
        public decimal InsuranceFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public string PromoCode { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
