using System;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace SchoolV01.Application.Features.Maintenances.Queries
{
    public class GetMaintenanceByIdQuery : IRequest<Result<GetAllMaintenancesResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetMaintenanceByIdQueryHandler : IRequestHandler<GetMaintenanceByIdQuery, Result<GetAllMaintenancesResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetMaintenanceByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllMaintenancesResponse>> Handle(GetMaintenanceByIdQuery query, CancellationToken cancellationToken)
        {
            var Maintenance = await _unitOfWork.Repository<Maintenance>().GetByIdAsync(query.Id);
            var mappedMaintenance = _mapper.Map<GetAllMaintenancesResponse>(Maintenance);
            return await Result<GetAllMaintenancesResponse>.SuccessAsync(mappedMaintenance);
        }
    }
}