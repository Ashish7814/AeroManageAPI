using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Entities
{
    public class Invoice
    {
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string BookingReference { get; set; }
        public string PNR { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public BookingPricing Pricing { get; set; }
        public List<InvoiceLineItem> LineItems { get; set; }
    }
}
