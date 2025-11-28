using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Application.Interfaces.Services;
using SchoolV01.Application.Requests;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Constants.Application;
using SchoolV01.Shared.Wrapper;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolV01.Application.Features.Devices.Commands
{
    public partial class AddEditDeviceCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string EnglishName { get; set; } = "";

    
        public string LicenseUrl { get; set; }
        public UploadRequest IicenseUrlUploadRequest
        {
            get; set;

        }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Code { get; set; }
        public int? Year { get; set; }
        public DateTime? StartRun { get; set; }



        public int? ProjectTypeId { get; set; }
        public int? SubProjectTypeId { get; set; }

        public int? SubSubProjectTypeId { get; set; }

        public int? SupplierId { get; set; }

        public string ByType { get; set; }   // مشفى ولا عيادة
        public int? ClinicId { get; set; }
        public int? HospitalId { get; set; }
        public string DeviceStatus { get; set; }

        internal class AddEditDeviceCommandHandler : IRequestHandler<AddEditDeviceCommand, Result<int>>
        {
            private readonly IMapper _mapper;
            private readonly IStringLocalizer<AddEditDeviceCommandHandler> _localizer;
            private readonly IUnitOfWork<int> _unitOfWork;
            private readonly IUploadService _uploadService;

            public AddEditDeviceCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IUploadService uploadService, IStringLocalizer<AddEditDeviceCommandHandler> localizer)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _localizer = localizer;
                _uploadService = uploadService;
            }

            public async Task<Result<int>> Handle(AddEditDeviceCommand command, CancellationToken cancellationToken)
            {

                var IicenseUrlUploadRequest = command.IicenseUrlUploadRequest;
                if (IicenseUrlUploadRequest != null)
                {
                    IicenseUrlUploadRequest.FileName = $"{Path.GetRandomFileName()}{IicenseUrlUploadRequest.Extension}";
                }

                if (command.Id == 0)
                {


                    var position = _mapper.Map<Device>(command);

                    if (IicenseUrlUploadRequest != null)
                    {
                        position.LicenseUrl = _uploadService.UploadAsync(IicenseUrlUploadRequest);
                    }

                    await _unitOfWork.Repository<Device>().AddAsync(position);

                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDevicesCacheKey);
                    return await Result<int>.SuccessAsync(position.Id, _localizer["Device Saved"]);
                }
                else
                {
                    var position = await _unitOfWork.Repository<Device>().GetByIdAsync(command.Id);
                    if (position != null)
                    {
                        position.Name = command.Name ?? position.Name;
                        position.ProjectTypeId = command.ProjectTypeId ?? position.ProjectTypeId;
                        position.SubProjectTypeId = command.SubProjectTypeId ?? position.SubProjectTypeId;
                        position.SubSubProjectTypeId = command.SubSubProjectTypeId ?? position.SubSubProjectTypeId;
                        position.Model = command.Model ?? position.Model;
                        position.SerialNumber = command.SerialNumber ?? position.SerialNumber;
                        position.Code = command.Code ?? position.Code;
                        position.Year = command.Year ?? position.Year;
                        position.DeviceStatus = command.DeviceStatus ?? position.DeviceStatus;
                        position.StartRun = command.StartRun ?? position.StartRun;
                        position.HospitalId = command.HospitalId ?? position.HospitalId;
                        position.ClinicId = command.ClinicId ?? position.ClinicId;
                        position.SupplierId = command.SupplierId ?? position.SupplierId;
                   
                        position.EnglishName = command.EnglishName ?? position.EnglishName;
                        if (IicenseUrlUploadRequest != null)
                        {
                            position.LicenseUrl = _uploadService.UploadAsync(IicenseUrlUploadRequest);
                        }
                        if (IicenseUrlUploadRequest == null)
                        {
                            position.LicenseUrl = command.LicenseUrl;
                        }
                        await _unitOfWork.Repository<Device>().UpdateAsync(position);
                        await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllDevicesCacheKey);
                        return await Result<int>.SuccessAsync(position.Id, _localizer["Device Updated"]);
                    }
                    else
                    {
                        return await Result<int>.FailAsync(_localizer["Device Not Found!"]);
                    }
                }
            }
        }
    }
}