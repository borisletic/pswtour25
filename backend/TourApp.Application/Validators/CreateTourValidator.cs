using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;

namespace TourApp.Application.Validators
{
    public class CreateTourValidator : AbstractValidator<CreateTourCommand>
    {
        public CreateTourValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tour name is required")
                .MaximumLength(200).WithMessage("Tour name cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Tour description is required")
                .MaximumLength(2000).WithMessage("Tour description cannot exceed 2000 characters");

            RuleFor(x => x.Difficulty)
                .IsInEnum().WithMessage("Invalid tour difficulty");

            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Invalid tour category");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero");

            RuleFor(x => x.ScheduledDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Tour date must be in the future");
        }
    }
}
