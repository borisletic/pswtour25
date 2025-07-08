using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;

namespace TourApp.Application.Validators
{
    public class AddKeyPointValidator : AbstractValidator<AddKeyPointCommand>
    {
        public AddKeyPointValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Key point name is required")
                .MaximumLength(100).WithMessage("Key point name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Key point description is required")
                .MaximumLength(500).WithMessage("Key point description cannot exceed 500 characters");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");

            RuleFor(x => x.Order)
                .GreaterThan(0).WithMessage("Order must be greater than zero");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(500).WithMessage("Image URL cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));
        }
    }
}
