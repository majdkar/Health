using System;
using SchoolV01.Application.Features.ProjectTypes.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace SchoolV01.Application.Validators.Features.Positions.Commands.AddEdit
{
    public class AddEditProjectTypeCommandValidator : AbstractValidator<AddEditProjectTypeCommand>
    {
        public AddEditProjectTypeCommandValidator(IStringLocalizer<AddEditProjectTypeCommandValidator> localizer)
        {
			RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);
            RuleFor(request => request.EnglishName)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["English Name is required!"]);

        }
    }
}