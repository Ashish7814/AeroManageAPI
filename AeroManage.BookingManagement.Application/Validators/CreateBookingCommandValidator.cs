using AeroManage.BookingManagement.Application.Commands.Bookings;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AeroManage.BookingManagement.Application.Validators
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            RuleFor(x => x.dto).NotNull();
            RuleFor(x => x.dto.FlightIds).GreaterThan(0).When(x => x.Dto != null);
            RuleFor(x => x.dto.Passengers).NotEmpty().When(x => x.dto != null);
        }
    }
}
