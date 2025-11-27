using System;
using SchoolV01.Application.Features.Devices.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace SchoolV01.Application.Validators.Features.Devices.Commands.AddEdit
{
    public class AddEditDeviceCommandValidator : AbstractValidator<AddEditDeviceCommand>
    {
        public AddEditDeviceCommandValidator(IStringLocalizer<AddEditDeviceCommandValidator> localizer)
        {
			RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);

        }
    }
}