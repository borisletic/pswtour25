using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourApp.Application.Commands;

namespace TourApp.Application.Validators
{
    public class ReportProblemValidator : AbstractValidator<ReportProblemCommand>
    {
        public ReportProblemValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Problem title is required")
                .MaximumLength(200).WithMessage("Problem title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Problem description is required")
                .MaximumLength(1000).WithMessage("Problem description cannot exceed 1000 characters");
        }
    }
}
