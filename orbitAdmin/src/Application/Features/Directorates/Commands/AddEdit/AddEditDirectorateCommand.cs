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

namespace SchoolV01.Application.Features.Directorates.Commands
{
    public partial class AddEditDirectorateCommand : IRequest<Result<int>>
    {
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
        public string EnglishName { get; set; } = "";
        public int CityId { get; set; }

    }

    internal class AddEditDirectorateCommandHandler : IRequestHandler<AddEditDirectorateCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditDirectorateCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditDirectorateCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditDirectorateCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditDirectorateCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
                var position = _mapper.Map<Directorate>(command);
                await _unitOfWork.Repository<Directorate>().AddAsync(position);
             
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDirectoratesCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["Directorate Saved"]);
            }
            else
            {
                var position = await _unitOfWork.Repository<Directorate>().GetByIdAsync(command.Id);
                if (position != null)
                {
					position.Name = command.Name ?? position.Name;
					position.CityId = command.CityId;
					position.EnglishName = command.EnglishName ?? position.EnglishName;

                    await _unitOfWork.Repository<Directorate>().UpdateAsync(position);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDirectoratesCacheKey);
                    return await Result<int>.SuccessAsync(position.Id, _localizer["Directorate Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Directorate Not Found!"]);
                }
            }
        }
    }
}