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

namespace SchoolV01.Application.Features.Devices.Queries
{
    public class GetAllPagedDevicesBySupplierIdQuery : IRequest<PaginatedResult<GetAllDevicesResponse>>
    {
      
        public int SupplierId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllPagedDevicesBySupplierIdQuery(int supplierId, int pageNumber, int pageSize, string searchString, string orderBy)
        {
            SupplierId = supplierId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllPagedDevicesBySupplierIdQueryHandler : IRequestHandler<GetAllPagedDevicesBySupplierIdQuery, PaginatedResult<GetAllDevicesResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllPagedDevicesBySupplierIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PaginatedResult<GetAllDevicesResponse>> Handle(GetAllPagedDevicesBySupplierIdQuery request, CancellationToken cancellationToken)
        {
            var DeviceFilterSpec = new DeviceBySupplierIdFilterSpecification(request.SearchString,request.SupplierId);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<Device>().Entities
                    .Specify(DeviceFilterSpec)
                    .SelectDevice()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
                var data = await _unitOfWork.Repository<Device>().Entities
                    .Specify(DeviceFilterSpec)
                    .OrderBy(ordering) // require system.linq.dynamic.core
                    .SelectDevice()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
        }
    }
}