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

namespace SchoolV01.Application.Features.Directorates.Commands
{
    public class DeleteDirectorateCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteDirectorateCommandHandler : IRequestHandler<DeleteDirectorateCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteDirectorateCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteDirectorateCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteDirectorateCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteDirectorateCommand command, CancellationToken cancellationToken)
        {
            var position = await _unitOfWork.Repository<Directorate>().GetByIdAsync(command.Id);
          
            if (position != null)
            {
                position.Deleted = true;
                await _unitOfWork.Repository<Directorate>().UpdateAsync(position);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDirectoratesCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["Directorate Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Directorate Not Found!"]);
            }
        }
    }
}