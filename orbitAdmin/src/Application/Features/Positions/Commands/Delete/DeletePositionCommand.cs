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

namespace SchoolV01.Application.Features.Positions.Commands
{
    public class DeletePositionCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeletePositionCommandHandler : IRequestHandler<DeletePositionCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeletePositionCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeletePositionCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeletePositionCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeletePositionCommand command, CancellationToken cancellationToken)
        {
            var position = await _unitOfWork.Repository<Position>().GetByIdAsync(command.Id);
          
            if (position != null)
            {
                position.Deleted = true;
                await _unitOfWork.Repository<Position>().UpdateAsync(position);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllPositionsCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["Position Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Position Not Found!"]);
            }
        }
    }
}