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

namespace SchoolV01.Application.Features.Devices.Commands
{
    public class DeleteDeviceCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteDeviceCommandHandler : IRequestHandler<DeleteDeviceCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteDeviceCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteDeviceCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteDeviceCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteDeviceCommand command, CancellationToken cancellationToken)
        {
            var position = await _unitOfWork.Repository<Device>().GetByIdAsync(command.Id);
          
            if (position != null)
            {
                position.Deleted = true;
                await _unitOfWork.Repository<Device>().UpdateAsync(position);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDevicesCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["Device Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Device Not Found!"]);
            }
        }
    }
}