using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Domain.Enums
{
    public enum BookingStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3,
        Expired = 4
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Processing = 1,
        Paid = 2,
        Failed = 3,
        Refunded = 4,
        PartiallyRefunded = 5
    }

    public enum SeatClass
    {
        Economy = 0,
        Business = 1,
        FirstClass = 2
    }

    public enum AddOnType
    {
        ExtraBaggage = 0,
        //Travel Insurance = 1,
        Meal = 2,
        LoungeAccess = 3,
        PriorityBoarding = 4,
        WiFi = 5
    }

    public enum PaymentMethod
    {
        CreditCard = 0,
        DebitCard = 1,
        PayPal = 2,
        GooglePay = 3,
        ApplePay = 4,
        BankTransfer = 5,
        PayLater = 6
    }

}
