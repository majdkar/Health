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

namespace SchoolV01.Application.Features.Clinics.Commands
{
    public partial class AddEditClinicCommand : IRequest<Result<int>>
    {
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
        public string EnglishName { get; set; } = "";
        public int CityId { get; set; }
        public bool ByDirectorate { get; set; }

        public int? DirectorateId { get; set; }
    }

    internal class AddEditClinicCommandHandler : IRequestHandler<AddEditClinicCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditClinicCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditClinicCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditClinicCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditClinicCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
                var position = _mapper.Map<Clinic>(command);
                await _unitOfWork.Repository<Clinic>().AddAsync(position);
             
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllClinicsCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["Clinic Saved"]);
            }
            else
            {
                var position = await _unitOfWork.Repository<Clinic>().GetByIdAsync(command.Id);
                if (position != null)
                {
					position.Name = command.Name ?? position.Name;
					position.CityId = command.CityId;
					position.ByDirectorate = command.ByDirectorate;
					position.DirectorateId = command.DirectorateId;
					position.EnglishName = command.EnglishName ?? position.EnglishName;

                    await _unitOfWork.Repository<Clinic>().UpdateAsync(position);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllClinicsCacheKey);
                    return await Result<int>.SuccessAsync(position.Id, _localizer["Clinic Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Clinic Not Found!"]);
                }
            }
        }
    }
}