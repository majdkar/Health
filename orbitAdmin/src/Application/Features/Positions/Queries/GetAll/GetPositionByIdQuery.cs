using System;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace SchoolV01.Application.Features.Positions.Queries
{
    public class GetPositionByIdQuery : IRequest<Result<GetAllPositionsResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetPositionByIdQueryHandler : IRequestHandler<GetPositionByIdQuery, Result<GetAllPositionsResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetPositionByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllPositionsResponse>> Handle(GetPositionByIdQuery query, CancellationToken cancellationToken)
        {
            var Position = await _unitOfWork.Repository<Position>().GetByIdAsync(query.Id);
            var mappedPosition = _mapper.Map<GetAllPositionsResponse>(Position);
            return await Result<GetAllPositionsResponse>.SuccessAsync(mappedPosition);
        }
    }
}