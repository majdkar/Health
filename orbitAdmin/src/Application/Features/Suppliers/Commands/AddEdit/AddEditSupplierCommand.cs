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

namespace SchoolV01.Application.Features.Suppliers.Commands
{
    public partial class AddEditSupplierCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string EnglishName { get; set; } = "";

        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public int? CountryId { get; set; }
        public int? PositionId { get; set; }

        public string LicenseUrl { get; set; }
        public UploadRequest IicenseUrlUploadRequest
        {
            get; set;

        }

        internal class AddEditSupplierCommandHandler : IRequestHandler<AddEditSupplierCommand, Result<int>>
        {
            private readonly IMapper _mapper;
            private readonly IStringLocalizer<AddEditSupplierCommandHandler> _localizer;
            private readonly IUnitOfWork<int> _unitOfWork;
            private readonly IUploadService _uploadService;

            public AddEditSupplierCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IUploadService uploadService, IStringLocalizer<AddEditSupplierCommandHandler> localizer)
            {
                _unitOfWork = unitOfWork;
                _mapper = mapper;
                _localizer = localizer;
                _uploadService = uploadService;
            }

            public async Task<Result<int>> Handle(AddEditSupplierCommand command, CancellationToken cancellationToken)
            {

                var IicenseUrlUploadRequest = command.IicenseUrlUploadRequest;
                if (IicenseUrlUploadRequest != null)
                {
                    IicenseUrlUploadRequest.FileName = $"{Path.GetRandomFileName()}{IicenseUrlUploadRequest.Extension}";
                }

                if (command.Id == 0)
                {


                    var position = _mapper.Map<Supplier>(command);

                    if (IicenseUrlUploadRequest != null)
                    {
                        position.LicenseUrl = _uploadService.UploadAsync(IicenseUrlUploadRequest);
                    }

                    await _unitOfWork.Repository<Supplier>().AddAsync(position);

                    await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllSuppliersCacheKey);
                    return await Result<int>.SuccessAsync(position.Id, _localizer["Supplier Saved"]);
                }
                else
                {
                    var position = await _unitOfWork.Repository<Supplier>().GetByIdAsync(command.Id);
                    if (position != null)
                    {
                        position.Name = command.Name ?? position.Name;
                        position.CityId = command.CityId;
                        position.Address = command.Address ?? position.Address;
                        position.Email = command.Email ?? position.Email;
                        position.Mobile = command.Mobile ?? position.Mobile;
                        position.CountryId = command.CountryId;
                        position.CityId = command.CityId;
                        position.PositionId = command.PositionId;
                        position.EnglishName = command.EnglishName ?? position.EnglishName;
                        if (IicenseUrlUploadRequest != null)
                        {
                            position.LicenseUrl = _uploadService.UploadAsync(IicenseUrlUploadRequest);
                        }
                        if (IicenseUrlUploadRequest == null)
                        {
                            position.LicenseUrl = command.LicenseUrl;
                        }
                        await _unitOfWork.Repository<Supplier>().UpdateAsync(position);
                        await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllSuppliersCacheKey);
                        return await Result<int>.SuccessAsync(position.Id, _localizer["Supplier Updated"]);
                    }
                    else
                    {
                        return await Result<int>.FailAsync(_localizer["Supplier Not Found!"]);
                    }
                }
            }
        }
    }
}