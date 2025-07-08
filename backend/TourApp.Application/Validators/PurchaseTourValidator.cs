using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;

namespace TourApp.Application.Validators
{
    public class PurchaseTourValidator : AbstractValidator<PurchaseTourCommand>
    {
        public PurchaseTourValidator()
        {
            RuleFor(x => x.TourIds)
                .NotEmpty().WithMessage("At least one tour must be selected")
                .Must(x => x != null && x.Count > 0).WithMessage("At least one tour must be selected");

            RuleFor(x => x.BonusPointsToUse)
                .GreaterThanOrEqualTo(0).WithMessage("Bonus points cannot be negative");
        }
    }
}
