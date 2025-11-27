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

namespace SchoolV01.Application.Features.Hospitals.Queries
{
    public class GetAllHospitalsQuery : IRequest<Result<List<GetAllHospitalsResponse>>>
    {
        public GetAllHospitalsQuery()
        {
        }
    }

    internal class GetAllHospitalsCachedQueryHandler : IRequestHandler<GetAllHospitalsQuery, Result<List<GetAllHospitalsResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllHospitalsCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllHospitalsResponse>>> Handle(GetAllHospitalsQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<Hospital>>> getAllHospitals = () => _unitOfWork.Repository<Hospital>().GetAllAsync();
            var HospitalList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllHospitalsCacheKey, getAllHospitals);
            var mappedHospitals = _mapper.Map<List<GetAllHospitalsResponse>>(HospitalList);
            return await Result<List<GetAllHospitalsResponse>>.SuccessAsync(mappedHospitals);
        }
    }
}