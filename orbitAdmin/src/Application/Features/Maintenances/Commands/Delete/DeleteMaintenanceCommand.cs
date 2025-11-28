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

namespace SchoolV01.Application.Features.Maintenances.Commands
{
    public class DeleteMaintenanceCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteMaintenanceCommandHandler : IRequestHandler<DeleteMaintenanceCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteMaintenanceCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteMaintenanceCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteMaintenanceCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteMaintenanceCommand command, CancellationToken cancellationToken)
        {
            var position = await _unitOfWork.Repository<Maintenance>().GetByIdAsync(command.Id);
          
            if (position != null)
            {
                position.Deleted = true;
                await _unitOfWork.Repository<Maintenance>().UpdateAsync(position);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllMaintenancesCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["Maintenance Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Maintenance Not Found!"]);
            }
        }
    }
}