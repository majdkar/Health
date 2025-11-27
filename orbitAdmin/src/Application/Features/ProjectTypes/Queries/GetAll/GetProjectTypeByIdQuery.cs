using System;
using AutoMapper;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace SchoolV01.Application.Features.ProjectTypes.Queries
{
    public class GetProjectTypeByIdQuery : IRequest<Result<GetAllProjectTypesResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetProjectTypeByIdQueryHandler
        : IRequestHandler<GetProjectTypeByIdQuery, Result<GetAllProjectTypesResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IMapper _mapper;

        public GetProjectTypeByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<GetAllProjectTypesResponse>> Handle(
            GetProjectTypeByIdQuery query,
            CancellationToken cancellationToken)
        {
     
            var allTypes = await _unitOfWork.Repository<ProjectType>().GetAllAsync();

          
            var entity = allTypes.FirstOrDefault(x => x.Id == query.Id);

            if (entity == null)
                return await Result<GetAllProjectTypesResponse>.FailAsync("Project Type Not Found!");

          
            var mapped = _mapper.Map<List<GetAllProjectTypesResponse>>(allTypes);

          
            var lookup = mapped.ToDictionary(x => x.Id);

            
            foreach (var item in mapped)
            {
                if (item.ParentId.HasValue && lookup.ContainsKey(item.ParentId.Value))
                {
                    lookup[item.ParentId.Value].SubProjectTypes.Add(item);
                }
            }

            
            return await Result<GetAllProjectTypesResponse>.SuccessAsync(lookup[query.Id]);
        }
    }
}
