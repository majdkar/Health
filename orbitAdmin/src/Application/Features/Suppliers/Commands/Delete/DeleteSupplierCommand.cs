using System;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using SchoolV01.Shared.Constants.Application;
using Microsoft.EntityFrameworkCore;

namespace SchoolV01.Application.Features.Suppliers.Commands
{
    public class DeleteSupplierCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteSupplierCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteSupplierCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteSupplierCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteSupplierCommand command, CancellationToken cancellationToken)
        {
            var position = await _unitOfWork.Repository<Supplier>().GetByIdAsync(command.Id);
          
            if (position != null)
            {
                position.Deleted = true;
                await _unitOfWork.Repository<Supplier>().UpdateAsync(position);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllSuppliersCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["Supplier Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Supplier Not Found!"]);
            }
        }
    }
}