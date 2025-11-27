using System;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace SchoolV01.Application.Features.Suppliers.Queries
{
    public class GetSupplierByIdQuery : IRequest<Result<GetAllSuppliersResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, Result<GetAllSuppliersResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetSupplierByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllSuppliersResponse>> Handle(GetSupplierByIdQuery query, CancellationToken cancellationToken)
        {
            var Supplier = await _unitOfWork.Repository<Supplier>().GetByIdAsync(query.Id);
            var mappedSupplier = _mapper.Map<GetAllSuppliersResponse>(Supplier);
            return await Result<GetAllSuppliersResponse>.SuccessAsync(mappedSupplier);
        }
    }
}