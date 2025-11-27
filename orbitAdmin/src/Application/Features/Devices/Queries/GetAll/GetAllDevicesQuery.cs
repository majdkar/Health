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

namespace SchoolV01.Application.Features.Devices.Queries
{
    public class GetAllDevicesQuery : IRequest<Result<List<GetAllDevicesResponse>>>
    {
        public GetAllDevicesQuery()
        {
        }
    }

    internal class GetAllDevicesCachedQueryHandler : IRequestHandler<GetAllDevicesQuery, Result<List<GetAllDevicesResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllDevicesCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllDevicesResponse>>> Handle(GetAllDevicesQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<Device>>> getAllDevices = () => _unitOfWork.Repository<Device>().GetAllAsync();
            var DeviceList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllDevicesCacheKey, getAllDevices);
            var mappedDevices = _mapper.Map<List<GetAllDevicesResponse>>(DeviceList);
            return await Result<List<GetAllDevicesResponse>>.SuccessAsync(mappedDevices);
        }
    }
}