using System;
using SchoolV01.Application.Features.Suppliers.Commands;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace SchoolV01.Application.Validators.Features.Suppliers.Commands.AddEdit
{
    public class AddEditSupplierCommandValidator : AbstractValidator<AddEditSupplierCommand>
    {
        public AddEditSupplierCommandValidator(IStringLocalizer<AddEditSupplierCommandValidator> localizer)
        {
			RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);

        }
    }
}