using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;

using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using SchoolV01.Shared.Constants.Application;

namespace SchoolV01.Application.Features.DeviceStatuss.Commands
{
    public partial class AddEditDeviceStatusCommand : IRequest<Result<int>>
    {
		public int Id { get; set; }
		[Required]
        public DateTime? DeviceStatusDate { get; set; }
        public string Status { get; set; } = "";

        public int DeviceId { get; set; }
    }

    internal class AddEditDeviceStatusCommandHandler : IRequestHandler<AddEditDeviceStatusCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditDeviceStatusCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditDeviceStatusCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditDeviceStatusCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditDeviceStatusCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
                var position = _mapper.Map<DeviceStatus>(command);
                await _unitOfWork.Repository<DeviceStatus>().AddAsync(position);
             
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDeviceStatussCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["DeviceStatus Saved"]);
            }
            else
            {
                var position = await _unitOfWork.Repository<DeviceStatus>().GetByIdAsync(command.Id);
                if (position != null)
                {
					position.Status = command.Status ?? position.Status;
					position.DeviceStatusDate = command.DeviceStatusDate;
					position.DeviceId = command.DeviceId;
			

                    await _unitOfWork.Repository<DeviceStatus>().UpdateAsync(position);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDeviceStatussCacheKey);
                    return await Result<int>.SuccessAsync(position.Id, _localizer["DeviceStatus Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["DeviceStatus Not Found!"]);
                }
            }
        }
    }
}