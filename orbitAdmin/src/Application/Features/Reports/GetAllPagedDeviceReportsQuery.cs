using AutoMapper;
using SchoolV01.Application.Extensions;
using SchoolV01.Application.Interfaces.Repositories;

using SchoolV01.Domain.Enums;
using SchoolV01.Shared.Wrapper;
using LazyCache;
using MediatR;
using SchoolV01.Application.Enums;
using SchoolV01.Application.Responses.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SchoolV01.Application.Specifications.GeneralSettings;
using SchoolV01.Domain.Entities.GeneralSettings;

namespace SchoolV01.Application.Features.Reports
{
    public class GetAllPagedDeviceReportsQuery : IRequest<PaginatedResult<GetAllDeviceReportsResponse>>
    {
        public string DeviceNameAr { get; set; }
        public string DeviceNameEn { get; set; }
        public string DeviceStatus { get; set; }
        public int ProjectTypeId { get; set; }
        public int SubProjectTypeId { get; set; }
        public int CityId { get; set; }
        public int ClinicId { get; set; }
        public int HospitalId { get; set; }
        public int DirectorateId { get; set; }
        public int Year { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? RunFrom { get; set; }
        public DateTime? RunTo { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; }

        public GetAllPagedDeviceReportsQuery(
            int pageNumber,
            int pageSize,
            string searchString,
            string deviceNameAr,
            string deviceNameEn,
            string deviceStatus,
            int projectTypeId,
            int subProjectTypeId,
            int cityId,
            int clinicId,
            int hospitalId,
            int directorateId,
            int year,
            string serialNumber,
            DateTime? runFrom,
            DateTime? runTo,
            string[] orderBy)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;

            DeviceNameAr = deviceNameAr;
            DeviceNameEn = deviceNameEn;
            DeviceStatus = deviceStatus;
            ProjectTypeId = projectTypeId;
            SubProjectTypeId = subProjectTypeId;
            CityId = cityId;
            ClinicId = clinicId;
            HospitalId = hospitalId;
            DirectorateId = directorateId;
            Year = year;
            SerialNumber = serialNumber;
            RunFrom = runFrom;
            RunTo = runTo;

            OrderBy = orderBy;
        }
    }


    internal class GetAllPagedDeviceReportsQueryHandler : IRequestHandler<GetAllPagedDeviceReportsQuery, PaginatedResult<GetAllDeviceReportsResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllPagedDeviceReportsQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PaginatedResult<GetAllDeviceReportsResponse>> Handle(GetAllPagedDeviceReportsQuery request, CancellationToken cancellationToken)
        {
            var SupplierFilterSpec = new ReportFilterSpecification(request);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<Device>().Entities
                    .Specify(SupplierFilterSpec)
                    .SelectReport()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
                var data = await _unitOfWork.Repository<Device>().Entities
                    .Specify(SupplierFilterSpec)
                    .OrderBy(ordering) // require system.linq.dynamic.core
                    .SelectReport()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
        }
    }
}