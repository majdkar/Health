using System;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SchoolV01.Application.Features.Devices.Queries
{
    public class GetDeviceByIdQuery : IRequest<Result<GetByIdDevicesResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetDeviceByIdQueryHandler : IRequestHandler<GetDeviceByIdQuery, Result<GetByIdDevicesResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetDeviceByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetByIdDevicesResponse>> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken)
        {
            var device = await _unitOfWork.Repository<Device>().Entities.Include(x => x.ProjectType)
                .Include(x => x.SubProjectType)
                .Include(x => x.SubSubProjectType)
                .Include(x => x.Supplier)
                .Include(x => x.Clinic)
                .Include(x => x.Hospital).FirstOrDefaultAsync(x => x.Id == request.Id);

            var mapped = _mapper.Map<GetByIdDevicesResponse>(device);

            return await Result<GetByIdDevicesResponse>.SuccessAsync(mapped);
        }
    }
}