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

namespace SchoolV01.Application.Features.ProjectTypes.Commands
{
    public partial class AddEditProjectTypeCommand : IRequest<Result<int>>
    {
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
        public string EnglishName { get; set; } = "";
        public int? ParentId { get; set; }
    }

    internal class AddEditProjectTypeCommandHandler : IRequestHandler<AddEditProjectTypeCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditProjectTypeCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditProjectTypeCommandHandler(
            IUnitOfWork<int> unitOfWork,
            IMapper mapper,
            IStringLocalizer<AddEditProjectTypeCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditProjectTypeCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
              
                var entity = _mapper.Map<ProjectType>(command);

                await _unitOfWork.Repository<ProjectType>().AddAsync(entity);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken,ApplicationConstants.Cache.GetAllProjectTypesCacheKey);

                return await Result<int>.SuccessAsync(entity.Id, _localizer["Project Type Saved"]);
            }
            else
            {
                var entity = await _unitOfWork.Repository<ProjectType>().GetByIdAsync(command.Id);

                if (entity == null)
                    return await Result<int>.FailAsync(_localizer["Project Type Not Found!"]);

                entity.Name = command.Name ?? entity.Name;
                entity.EnglishName = command.EnglishName ?? entity.EnglishName;

               
                entity.ParentId = command.ParentId;

                await _unitOfWork.Repository<ProjectType>().UpdateAsync(entity);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllProjectTypesCacheKey);

                return await Result<int>.SuccessAsync(entity.Id, _localizer["Project Type Updated"]);
            }
        }
    }

}