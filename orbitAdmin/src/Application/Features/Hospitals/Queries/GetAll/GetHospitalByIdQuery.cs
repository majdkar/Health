using System;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace SchoolV01.Application.Features.Hospitals.Queries
{
    public class GetHospitalByIdQuery : IRequest<Result<GetAllHospitalsResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetHospitalByIdQueryHandler : IRequestHandler<GetHospitalByIdQuery, Result<GetAllHospitalsResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetHospitalByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllHospitalsResponse>> Handle(GetHospitalByIdQuery query, CancellationToken cancellationToken)
        {
            var Hospital = await _unitOfWork.Repository<Hospital>().GetByIdAsync(query.Id);
            var mappedHospital = _mapper.Map<GetAllHospitalsResponse>(Hospital);
            return await Result<GetAllHospitalsResponse>.SuccessAsync(mappedHospital);
        }
    }
}