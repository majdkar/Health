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

namespace SchoolV01.Application.Features.Clinics.Queries
{
    public class GetAllPagedClinicsByDirectorateIdQuery : IRequest<PaginatedResult<GetAllClinicsResponse>>
    {
      
        public int DirectorateId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchString { get; set; }
        public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

        public GetAllPagedClinicsByDirectorateIdQuery(int directorateId ,int pageNumber, int pageSize, string searchString, string orderBy)
        {
            DirectorateId = directorateId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchString = searchString;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                OrderBy = orderBy.Split(',');
            }
        }
    }

    internal class GetAllPagedClinicsByDirectorateIdQueryHandler : IRequestHandler<GetAllPagedClinicsByDirectorateIdQuery, PaginatedResult<GetAllClinicsResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllPagedClinicsByDirectorateIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<PaginatedResult<GetAllClinicsResponse>> Handle(GetAllPagedClinicsByDirectorateIdQuery request, CancellationToken cancellationToken)
        {
            var ClinicFilterSpec = new ClinicByDirectorateIdFilterSpecification(request.SearchString,request.DirectorateId );
            if (request.OrderBy?.Any() != true)
            {
                var data = await _unitOfWork.Repository<Clinic>().Entities
                    .Specify(ClinicFilterSpec)
                    .SelectClinic()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
            else
            {
                var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
                var data = await _unitOfWork.Repository<Clinic>().Entities
                    .Specify(ClinicFilterSpec)
                    .OrderBy(ordering) // require system.linq.dynamic.core
                    .SelectClinic()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return data;
            }
        }
    }
}