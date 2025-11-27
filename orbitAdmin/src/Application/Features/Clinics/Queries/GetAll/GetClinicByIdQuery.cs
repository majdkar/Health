using System;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace SchoolV01.Application.Features.Clinics.Queries
{
    public class GetClinicByIdQuery : IRequest<Result<GetAllClinicsResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetClinicByIdQueryHandler : IRequestHandler<GetClinicByIdQuery, Result<GetAllClinicsResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetClinicByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllClinicsResponse>> Handle(GetClinicByIdQuery query, CancellationToken cancellationToken)
        {
            var Clinic = await _unitOfWork.Repository<Clinic>().GetByIdAsync(query.Id);
            var mappedClinic = _mapper.Map<GetAllClinicsResponse>(Clinic);
            return await Result<GetAllClinicsResponse>.SuccessAsync(mappedClinic);
        }
    }
}