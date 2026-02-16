using SchoolV01.Application.Features.FormCompanies.Queries;
using SchoolV01.Application.Features.FormCompanys.Commands;
using SchoolV01.Application.Requests.FormCompanies;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public interface IFormCompanyManager : IManager
    {
        
        Task<PaginatedResult<GetAllFormCompaniesResponse>> GetAllPagedAsync(GetAllPagedFormCompaniesRequest request);

        Task<IResult<int>> SaveAsync(AddEditFormCompanyCommand request);

        Task<IResult<int>> DeleteAsync(int id);
        Task<IResult<GetByIdFormCompaniesResponse>> GetAsync(int id);

    }
}