using System;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace SchoolV01.Application.Features.Directorates.Queries
{
    public class GetDirectorateByIdQuery : IRequest<Result<GetAllDirectoratesResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetDirectorateByIdQueryHandler : IRequestHandler<GetDirectorateByIdQuery, Result<GetAllDirectoratesResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetDirectorateByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllDirectoratesResponse>> Handle(GetDirectorateByIdQuery query, CancellationToken cancellationToken)
        {
            var Directorate = await _unitOfWork.Repository<Directorate>().GetByIdAsync(query.Id);
            var mappedDirectorate = _mapper.Map<GetAllDirectoratesResponse>(Directorate);
            return await Result<GetAllDirectoratesResponse>.SuccessAsync(mappedDirectorate);
        }
    }
}