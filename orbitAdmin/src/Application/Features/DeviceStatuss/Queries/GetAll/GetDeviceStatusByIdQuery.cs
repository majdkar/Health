using System;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace SchoolV01.Application.Features.DeviceStatuss.Queries
{
    public class GetDeviceStatusByIdQuery : IRequest<Result<GetAllDeviceStatussResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetDeviceStatusByIdQueryHandler : IRequestHandler<GetDeviceStatusByIdQuery, Result<GetAllDeviceStatussResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetDeviceStatusByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllDeviceStatussResponse>> Handle(GetDeviceStatusByIdQuery query, CancellationToken cancellationToken)
        {
            var DeviceStatus = await _unitOfWork.Repository<DeviceStatus>().GetByIdAsync(query.Id);
            var mappedDeviceStatus = _mapper.Map<GetAllDeviceStatussResponse>(DeviceStatus);
            return await Result<GetAllDeviceStatussResponse>.SuccessAsync(mappedDeviceStatus);
        }
    }
}