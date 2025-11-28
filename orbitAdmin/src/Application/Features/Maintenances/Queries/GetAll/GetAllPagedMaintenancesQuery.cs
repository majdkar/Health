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

namespace SchoolV01.Application.Features.Maintenances.Queries
{
    public class GetAllPagedMaintenancesQuery : IRequest<PaginatedResult<GetAllMaintenancesResponse>>
    {
        public int DeviceId { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllPagedMaintenancesQuery(int deviceId ,int pageNumber, int pageSize, string searchString, string orderBy)
        {
            DeviceId = deviceId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllPagedMaintenancesQueryHandler : IRequestHandler<GetAllPagedMaintenancesQuery, PaginatedResult<GetAllMaintenancesResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllPagedMaintenancesQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PaginatedResult<GetAllMaintenancesResponse>> Handle(GetAllPagedMaintenancesQuery request, CancellationToken cancellationToken)
        {
            var MaintenanceFilterSpec = new MaintenanceFilterSpecification(request.SearchString,request.DeviceId);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<Maintenance>().Entities
                    .Specify(MaintenanceFilterSpec)
                    .SelectMaintenance()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
                var data = await _unitOfWork.Repository<Maintenance>().Entities
                    .Specify(MaintenanceFilterSpec)
                    .OrderBy(ordering) // require system.linq.dynamic.core
                    .SelectMaintenance()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
        }
    }
}