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

namespace SchoolV01.Application.Features.FormCompanies.Queries
{
    public class GetAllPagedFormCompaniesQuery : IRequest<PaginatedResult<GetAllFormCompaniesResponse>>
    {
      
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllPagedFormCompaniesQuery( int pageNumber, int pageSize, string searchString, string orderBy)
        {
           
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllPagedFormCompaniesQueryHandler : IRequestHandler<GetAllPagedFormCompaniesQuery, PaginatedResult<GetAllFormCompaniesResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllPagedFormCompaniesQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PaginatedResult<GetAllFormCompaniesResponse>> Handle(GetAllPagedFormCompaniesQuery request, CancellationToken cancellationToken)
        {
            var FormCompanieFilterSpec = new FormCompanyFilterSpecification(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<FormCompany>().Entities
                    .Specify(FormCompanieFilterSpec)
                    .SelectFormCompany()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
                var data = await _unitOfWork.Repository<FormCompany>().Entities
                    .Specify(FormCompanieFilterSpec)
                    .OrderBy(ordering) // require system.linq.dynamic.core
                    .SelectFormCompany()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
        }
    }
}