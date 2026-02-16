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

namespace SchoolV01.Application.Features.FormCompanys.Commands
{
    public class DeleteFormCompanyCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
    }

    internal class DeleteFormCompanyCommandHandler : IRequestHandler<DeleteFormCompanyCommand, Result<int>>
    {
        private readonly IStringLocalizer<DeleteFormCompanyCommandHandler> _localizer;
        private readonly IUnitOfWork<int> _unitOfWork;

        public DeleteFormCompanyCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteFormCompanyCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(DeleteFormCompanyCommand command, CancellationToken cancellationToken)
        {
            var position = await _unitOfWork.Repository<FormCompany>().GetByIdAsync(command.Id);
          
            if (position != null)
            {
                position.Deleted = true;
                await _unitOfWork.Repository<FormCompany>().DeleteAsync(position);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFormCompanysCacheKey);
                return await Result<int>.SuccessAsync(position.Id, _localizer["Form Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(_localizer["Form Not Found!"]);
            }
        }
    }
}