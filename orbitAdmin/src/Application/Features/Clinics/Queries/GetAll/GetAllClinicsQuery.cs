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

namespace SchoolV01.Application.Features.Clinics.Queries
{
    public class GetAllClinicsQuery : IRequest<Result<List<GetAllClinicsResponse>>>
    {
        public GetAllClinicsQuery()
        {
        }
    }

    internal class GetAllClinicsCachedQueryHandler : IRequestHandler<GetAllClinicsQuery, Result<List<GetAllClinicsResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllClinicsCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllClinicsResponse>>> Handle(GetAllClinicsQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<Clinic>>> getAllClinics = () => _unitOfWork.Repository<Clinic>().GetAllAsync();
            var ClinicList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllClinicsCacheKey, getAllClinics);
            var mappedClinics = _mapper.Map<List<GetAllClinicsResponse>>(ClinicList);
            return await Result<List<GetAllClinicsResponse>>.SuccessAsync(mappedClinics);
        }
    }
}