using System;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace SchoolV01.Application.Features.Devices.Queries
{
    public class GetDeviceByIdQuery : IRequest<Result<GetAllDevicesResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetDeviceByIdQueryHandler : IRequestHandler<GetDeviceByIdQuery, Result<GetAllDevicesResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetDeviceByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllDevicesResponse>> Handle(GetDeviceByIdQuery query, CancellationToken cancellationToken)
        {
            var Device = await _unitOfWork.Repository<Device>().GetByIdAsync(query.Id);
            var mappedDevice = _mapper.Map<GetAllDevicesResponse>(Device);
            return await Result<GetAllDevicesResponse>.SuccessAsync(mappedDevice);
        }
    }
}