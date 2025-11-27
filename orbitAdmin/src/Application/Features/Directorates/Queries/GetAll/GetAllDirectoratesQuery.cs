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

namespace SchoolV01.Application.Features.Directorates.Queries
{
    public class GetAllDirectoratesQuery : IRequest<Result<List<GetAllDirectoratesResponse>>>
    {
        public GetAllDirectoratesQuery()
        {
        }
    }

    internal class GetAllDirectoratesCachedQueryHandler : IRequestHandler<GetAllDirectoratesQuery, Result<List<GetAllDirectoratesResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllDirectoratesCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllDirectoratesResponse>>> Handle(GetAllDirectoratesQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<Directorate>>> getAllDirectorates = () => _unitOfWork.Repository<Directorate>().GetAllAsync();
            var DirectorateList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllDirectoratesCacheKey, getAllDirectorates);
            var mappedDirectorates = _mapper.Map<List<GetAllDirectoratesResponse>>(DirectorateList);
            return await Result<List<GetAllDirectoratesResponse>>.SuccessAsync(mappedDirectorates);
        }
    }
}