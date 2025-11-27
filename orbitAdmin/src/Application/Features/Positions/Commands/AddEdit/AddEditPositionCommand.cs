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

namespace SchoolV01.Application.Features.Positions.Commands
{
    public partial class AddEditPositionCommand : IRequest<Result<int>>
    {
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
        public string EnglishName { get; set; } = "";

    }

    internal class AddEditPositionCommandHandler : IRequestHandler<AddEditPositionCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AddEditPositionCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public AddEditPositionCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IStringLocalizer<AddEditPositionCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditPositionCommand command, CancellationToken cancellationToken)
        {
            if (command.Id == 0)
            {
                var position = _mapper.Map<Position>(command);
                await _unitOfWork.Repository<Position>().AddAsync(position);
             
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllPositionsCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["Position Saved"]);
            }
            else
            {
                var position = await _unitOfWork.Repository<Position>().GetByIdAsync(command.Id);
                if (position != null)
                {
					position.Name = command.Name ?? position.Name;
					position.EnglishName = command.EnglishName ?? position.EnglishName;

                    await _unitOfWork.Repository<Position>().UpdateAsync(position);
                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllPositionsCacheKey);
                    return await Result<int>.SuccessAsync(position.Id, _localizer["Position Updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Position Not Found!"]);
                }
            }
        }
    }
}