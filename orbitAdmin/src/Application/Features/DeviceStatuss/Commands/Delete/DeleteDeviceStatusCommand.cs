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

namespace SchoolV01.Application.Features.DeviceStatuss.Commands
{
    public class DeleteDeviceStatusCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteDeviceStatusCommandHandler : IRequestHandler<DeleteDeviceStatusCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteDeviceStatusCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteDeviceStatusCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteDeviceStatusCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteDeviceStatusCommand command, CancellationToken cancellationToken)
        {
            var position = await _unitOfWork.Repository<DeviceStatus>().GetByIdAsync(command.Id);
          
            if (position != null)
            {
                position.Deleted = true;
                await _unitOfWork.Repository<DeviceStatus>().UpdateAsync(position);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDeviceStatussCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["DeviceStatus Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["DeviceStatus Not Found!"]);
            }
        }
    }
}