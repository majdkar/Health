using System;
using SchoolV01.Application.Features.Hospitals.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace SchoolV01.Application.Validators.Features.Hospitals.Commands.AddEdit
{
    public class AddEditHospitalCommandValidator : AbstractValidator<AddEditHospitalCommand>
    {
        public AddEditHospitalCommandValidator(IStringLocalizer<AddEditHospitalCommandValidator> localizer)
        {
			RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);

        }
    }
}