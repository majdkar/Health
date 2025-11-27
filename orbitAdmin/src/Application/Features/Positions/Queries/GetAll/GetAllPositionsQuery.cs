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

namespace SchoolV01.Application.Features.Positions.Queries
{
    public class GetAllPositionsQuery : IRequest<Result<List<GetAllPositionsResponse>>>
    {
        public GetAllPositionsQuery()
        {
        }
    }

    internal class GetAllPositionsCachedQueryHandler : IRequestHandler<GetAllPositionsQuery, Result<List<GetAllPositionsResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllPositionsCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllPositionsResponse>>> Handle(GetAllPositionsQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<Position>>> getAllPositions = () => _unitOfWork.Repository<Position>().GetAllAsync();
            var PositionList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllPositionsCacheKey, getAllPositions);
            var mappedPositions = _mapper.Map<List<GetAllPositionsResponse>>(PositionList);
            return await Result<List<GetAllPositionsResponse>>.SuccessAsync(mappedPositions);
        }
    }
}