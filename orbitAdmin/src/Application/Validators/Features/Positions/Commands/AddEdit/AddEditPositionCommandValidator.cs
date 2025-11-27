using System;
using SchoolV01.Application.Features.Positions.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace SchoolV01.Application.Validators.Features.Positions.Commands.AddEdit
{
    public class AddEditPositionCommandValidator : AbstractValidator<AddEditPositionCommand>
    {
        public AddEditPositionCommandValidator(IStringLocalizer<AddEditPositionCommandValidator> localizer)
        {
			RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);

        }
    }
}