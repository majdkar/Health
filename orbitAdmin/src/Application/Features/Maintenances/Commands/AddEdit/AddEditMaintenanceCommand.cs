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

namespace SchoolV01.Application.Features.Maintenances.Commands
{
    public partial class AddEditMaintenanceCommand : IRequest<Result<int>>
    {
		public int Id { get; set; }
		[Required]
        public DateTime? MaintenanceDate { get; set; }
        public string Description { get; set; } = "";

        public int DeviceId { get; set; }
    }

    internal class AddEditMaintenanceCommandHandler : IRequestHandler<AddEditMaintenanceCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditMaintenanceCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditMaintenanceCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditMaintenanceCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditMaintenanceCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
                var position = _mapper.Map<Maintenance>(command);
                await _unitOfWork.Repository<Maintenance>().AddAsync(position);
             
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllMaintenancesCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["Maintenance Saved"]);
            }
            else
            {
                var position = await _unitOfWork.Repository<Maintenance>().GetByIdAsync(command.Id);
                if (position != null)
                {
					position.Description = command.Description ?? position.Description;
					position.MaintenanceDate = command.MaintenanceDate;
					position.DeviceId = command.DeviceId;
			

                    await _unitOfWork.Repository<Maintenance>().UpdateAsync(position);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllMaintenancesCacheKey);
                    return await Result<int>.SuccessAsync(position.Id, _localizer["Maintenance Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Maintenance Not Found!"]);
                }
            }
        }
    }
}