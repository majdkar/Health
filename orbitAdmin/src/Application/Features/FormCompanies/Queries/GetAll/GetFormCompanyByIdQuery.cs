using System;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SchoolV01.Application.Features.FormCompanies.Queries
{
    public class GetFormCompanyByIdQuery : IRequest<Result<GetByIdFormCompaniesResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetFormCompanyByIdQueryHandler : IRequestHandler<GetFormCompanyByIdQuery, Result<GetByIdFormCompaniesResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetFormCompanyByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetByIdFormCompaniesResponse>> Handle(GetFormCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var device = await _unitOfWork.Repository<FormCompany>()
    .Entities
    .Include(x => x.Attachments) // 🔹 مهم جدًا
    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (device == null)
                return await Result<GetByIdFormCompaniesResponse>.FailAsync("Form not found");

            var mapped = _mapper.Map<GetByIdFormCompaniesResponse>(device);

            return await Result<GetByIdFormCompaniesResponse>.SuccessAsync(mapped);
        }
    }
}