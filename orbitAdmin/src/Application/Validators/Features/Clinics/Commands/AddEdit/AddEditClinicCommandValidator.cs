using System;
using SchoolV01.Application.Features.Clinics.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace SchoolV01.Application.Validators.Features.Clinics.Commands.AddEdit
{
    public class AddEditClinicCommandValidator : AbstractValidator<AddEditClinicCommand>
    {
        public AddEditClinicCommandValidator(IStringLocalizer<AddEditClinicCommandValidator> localizer)
        {
			RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);

        }
    }
}