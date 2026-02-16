using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Application.Interfaces.Services;
using SchoolV01.Application.Requests;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Constants.Application;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolV01.Application.Features.FormCompanys.Commands
{
    public partial class AddEditFormCompanyCommand : IRequest<Result<int>>
    {
            public int Id { get; set; }

            public string CompanyName { get; set; }
        public string FormNumber { get; set; }

        public string AgentName { get; set; }
            public string DeviceType { get; set; }
            public string DeviceBrand { get; set; }
            public string Model { get; set; }
            public List<UploadRequest> Attachments { get; set; } = new();
        }
        internal class AddEditFormCompanyCommandHandler : IRequestHandler<AddEditFormCompanyCommand, Result<int>>
        {
            private readonly IMapper _mapper;
            private readonly IStringLocalizer<AddEditFormCompanyCommandHandler> _localizer;
            private readonly IUnitOfWork<int> _unitOfWork;
            private readonly IUploadService _uploadService;

            public AddEditFormCompanyCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IUploadService uploadService, IStringLocalizer<AddEditFormCompanyCommandHandler> localizer)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _localizer = localizer;
                _uploadService = uploadService;
            }

            public async Task<Result<int>> Handle(AddEditFormCompanyCommand command, CancellationToken cancellationToken)
            {



            if (command.Id == 0)
            {
                var entity = _mapper.Map<FormCompany>(command);

                // حفظ الـ FormCompany أولاً للحصول على Id
                await _unitOfWork.Repository<FormCompany>().AddAsync(entity);
                await _unitOfWork.Commit(cancellationToken);

                // توليد رقم FormNumber بعد الحصول على Id
                entity.FormNumber = $"F-{entity.Id.ToString("D5")}";
                await _unitOfWork.Repository<FormCompany>().UpdateAsync(entity);

                // إضافة المرفقات يدوياً
                if (command.Attachments?.Count > 0)
                {
                    foreach (var file in command.Attachments)
                    {
                        file.FileName ??= $"{Path.GetRandomFileName()}{file.Extension}";
                        var fileUrl = _uploadService.UploadAsync(file);

                        var attachment = new FormCompanyAttachment
                        {
                            FormCompanyId = entity.Id,
                            FileName = file.FileName,
                            FileUrl = fileUrl
                        };

                        await _unitOfWork.Repository<FormCompanyAttachment>().AddAsync(attachment);
                    }
                }

                await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllFormCompanysCacheKey);

                return await Result<int>.SuccessAsync(entity.Id, _localizer["Form Saved"]);
            }
            else
                {
                var entity = await _unitOfWork.Repository<FormCompany>()
  .GetByIdAsync(command.Id);

                if (entity == null)
                    return await Result<int>.FailAsync(_localizer["Form Not Found!"]);

                entity.CompanyName = command.CompanyName ?? entity.CompanyName;
                entity.DeviceBrand = command.DeviceBrand ?? entity.DeviceBrand;
                entity.DeviceType = command.DeviceType ?? entity.DeviceType;
                entity.Model = command.Model ?? entity.Model;
                entity.AgentName = command.AgentName ?? entity.AgentName;

                if (command.Attachments?.Count > 0)
                {
                    foreach (var file in command.Attachments)
                    {
                        //file.FileName = file.FileName ??  $"{Path.GetRandomFileName()}{file.Extension}";
                        var fileUrl =  _uploadService.UploadAsync(file);

                        var attachment = new FormCompanyAttachment
                        {
                            FormCompanyId = entity.Id,
                            FileName = file.FileName ?? $"{Path.GetRandomFileName()}{file.Extension}",
                            FileUrl = fileUrl
                        };

                        await _unitOfWork.Repository<FormCompanyAttachment>()
                            .AddAsync(attachment);
                    }
                }

                await _unitOfWork.Repository<FormCompany>().UpdateAsync(entity);

                await _unitOfWork.CommitAndRemoveCache(
                    cancellationToken,
                    ApplicationConstants.Cache.GetAllFormCompanysCacheKey);

                return await Result<int>.SuccessAsync(entity.Id, _localizer["Form Updated"]);
            }

            }
        }
}