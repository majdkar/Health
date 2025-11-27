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

namespace SchoolV01.Application.Features.ProjectTypes.Queries
{
    public class GetAllLevelsProjectTypesQuery : IRequest<Result<List<GetAllProjectTypesResponse>>>
    {
        public GetAllLevelsProjectTypesQuery()
        {
        }
    }

    internal class GetAllLevelsProjectTypesCachedQueryHandler
        : IRequestHandler<GetAllLevelsProjectTypesQuery, Result<List<GetAllProjectTypesResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllLevelsProjectTypesCachedQueryHandler(
            IUnitOfWork<int> unitOfWork,
            IMapper mapper,
            IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllProjectTypesResponse>>> Handle(
            GetAllLevelsProjectTypesQuery request,
            CancellationToken cancellationToken)
        {
            Func<Task<List<ProjectType>>> getAllTypes = () =>
                _unitOfWork.Repository<ProjectType>().GetAllAsync();

            var types = await _cache.GetOrAddAsync(
                ApplicationConstants.Cache.GetAllProjectTypesCacheKey,
                getAllTypes);

     
            var mapped = _mapper.Map<List<GetAllProjectTypesResponse>>(types);

  
            var lookup = mapped.ToDictionary(x => x.Id);

        
            foreach (var item in mapped)
            {
                if (item.ParentId.HasValue && lookup.ContainsKey(item.ParentId.Value))
                {
                    lookup[item.ParentId.Value].SubProjectTypes.Add(item);
                }
            }

           
            var rootNodes = mapped.ToList();

            return await Result<List<GetAllProjectTypesResponse>>.SuccessAsync(rootNodes);
        }
    }
}
