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

namespace SchoolV01.Application.Features.Hospitals.Commands
{
    public partial class AddEditHospitalCommand : IRequest<Result<int>>
    {
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
        public string EnglishName { get; set; } = "";
        public int CityId { get; set; }
        public bool ByDirectorate { get; set; }

        public int? DirectorateId { get; set; }
    }

    internal class AddEditHospitalCommandHandler : IRequestHandler<AddEditHospitalCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditHospitalCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditHospitalCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditHospitalCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditHospitalCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
                var position = _mapper.Map<Hospital>(command);
                await _unitOfWork.Repository<Hospital>().AddAsync(position);
             
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllHospitalsCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["Hospital Saved"]);
            }
            else
            {
                var position = await _unitOfWork.Repository<Hospital>().GetByIdAsync(command.Id);
                if (position != null)
                {
					position.Name = command.Name ?? position.Name;
					position.CityId = command.CityId;
					position.ByDirectorate = command.ByDirectorate;
					position.DirectorateId = command.DirectorateId;
					position.EnglishName = command.EnglishName ?? position.EnglishName;

                    await _unitOfWork.Repository<Hospital>().UpdateAsync(position);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllHospitalsCacheKey);
                    return await Result<int>.SuccessAsync(position.Id, _localizer["Hospital Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Hospital Not Found!"]);
                }
            }
        }
    }
}