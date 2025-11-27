using SchoolV01.Application.Features.Directorates.Commands;
using SchoolV01.Application.Features.Directorates.Queries;
using SchoolV01.Application.Requests.Directorates;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public interface IDirectorateManager : IManager
    {
        Task<IResult<List<GetAllDirectoratesResponse>>> GetAllAsync();
        
        Task<PaginatedResult<GetAllDirectoratesResponse>> GetAllPagedAsync(GetAllPagedDirectoratesRequest request);

        Task<IResult<int>> SaveAsync(AddEditDirectorateCommand request);

        Task<IResult<int>> DeleteAsync(int id);
        Task<IResult<GetAllDirectoratesResponse>> GetAsync(int id);

    }
}