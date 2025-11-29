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

namespace SchoolV01.Application.Features.DeviceStatuss.Queries
{
    public class GetAllDeviceStatussQuery : IRequest<Result<List<GetAllDeviceStatussResponse>>>
    {
        public GetAllDeviceStatussQuery()
        {
        }
    }

    internal class GetAllDeviceStatussCachedQueryHandler : IRequestHandler<GetAllDeviceStatussQuery, Result<List<GetAllDeviceStatussResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllDeviceStatussCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllDeviceStatussResponse>>> Handle(GetAllDeviceStatussQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<DeviceStatus>>> getAllDeviceStatuss = () => _unitOfWork.Repository<DeviceStatus>().GetAllAsync();
            var DeviceStatusList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllDeviceStatussCacheKey, getAllDeviceStatuss);
            var mappedDeviceStatuss = _mapper.Map<List<GetAllDeviceStatussResponse>>(DeviceStatusList);
            return await Result<List<GetAllDeviceStatussResponse>>.SuccessAsync(mappedDeviceStatuss);
        }
    }
}