using System;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Constants.Application;
using SchoolV01.Shared.Wrapper;
using LazyCache;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace SchoolV01.Application.Features.Maintenances.Queries
{
    public class GetAllMaintenancesQuery : IRequest<Result<List<GetAllMaintenancesResponse>>>
    {
        public GetAllMaintenancesQuery()
        {
        }
    }

    internal class GetAllMaintenancesCachedQueryHandler : IRequestHandler<GetAllMaintenancesQuery, Result<List<GetAllMaintenancesResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllMaintenancesCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllMaintenancesResponse>>> Handle(GetAllMaintenancesQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<Maintenance>>> getAllMaintenances = () => _unitOfWork.Repository<Maintenance>().GetAllAsync();
            var MaintenanceList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllMaintenancesCacheKey, getAllMaintenances);
            var mappedMaintenances = _mapper.Map<List<GetAllMaintenancesResponse>>(MaintenanceList);
            return await Result<List<GetAllMaintenancesResponse>>.SuccessAsync(mappedMaintenances);
        }
    }
}