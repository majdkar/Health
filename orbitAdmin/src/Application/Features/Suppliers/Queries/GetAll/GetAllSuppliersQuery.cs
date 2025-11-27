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

namespace SchoolV01.Application.Features.Suppliers.Queries
{
    public class GetAllSuppliersQuery : IRequest<Result<List<GetAllSuppliersResponse>>>
    {
        public GetAllSuppliersQuery()
        {
        }
    }

    internal class GetAllSuppliersCachedQueryHandler : IRequestHandler<GetAllSuppliersQuery, Result<List<GetAllSuppliersResponse>>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppCache _cache;

        public GetAllSuppliersCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Result<List<GetAllSuppliersResponse>>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
        {
            Func<Task<List<Supplier>>> getAllSuppliers = () => _unitOfWork.Repository<Supplier>().GetAllAsync();
            var SupplierList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllSuppliersCacheKey, getAllSuppliers);
            var mappedSuppliers = _mapper.Map<List<GetAllSuppliersResponse>>(SupplierList);
            return await Result<List<GetAllSuppliersResponse>>.SuccessAsync(mappedSuppliers);
        }
    }
}