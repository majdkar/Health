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

namespace SchoolV01.Application.Features.Clinics.Commands
{
    public class DeleteClinicCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteClinicCommandHandler : IRequestHandler<DeleteClinicCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteClinicCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteClinicCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteClinicCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteClinicCommand command, CancellationToken cancellationToken)
        {
            var position = await _unitOfWork.Repository<Clinic>().GetByIdAsync(command.Id);
          
            if (position != null)
            {
                position.Deleted = true;
                await _unitOfWork.Repository<Clinic>().UpdateAsync(position);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllClinicsCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["Clinic Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Clinic Not Found!"]);
            }
        }
    }
}