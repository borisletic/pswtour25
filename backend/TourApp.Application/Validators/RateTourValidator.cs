using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;

namespace TourApp.Application.Validators
{
    public class RateTourValidator : AbstractValidator<RateTourCommand>
    {
        public RateTourValidator()
        {
            RuleFor(x => x.Score)
                .InclusiveBetween(1, 5).WithMessage("Score must be between 1 and 5");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required for low ratings")
                .When(x => x.Score <= 2);

            RuleFor(x => x.Comment)
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Comment));
        }
    }
}
