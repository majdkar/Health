using System;
using SchoolV01.Application.Features.Directorates.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace SchoolV01.Application.Validators.Features.Directorates.Commands.AddEdit
{
    public class AddEditDirectorateCommandValidator : AbstractValidator<AddEditDirectorateCommand>
    {
        public AddEditDirectorateCommandValidator(IStringLocalizer<AddEditDirectorateCommandValidator> localizer)
        {
			RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);

        }
    }
}