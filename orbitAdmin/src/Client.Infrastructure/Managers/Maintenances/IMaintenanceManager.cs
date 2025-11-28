using SchoolV01.Application.Features.Maintenances.Commands;
using SchoolV01.Application.Features.Maintenances.Queries;
using SchoolV01.Application.Requests.Maintenances;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public interface IMaintenanceManager : IManager
    {
        Task<IResult<List<GetAllMaintenancesResponse>>> GetAllAsync();
        
        Task<PaginatedResult<GetAllMaintenancesResponse>> GetAllPagedAsync(GetAllPagedMaintenancesRequest request);

        Task<IResult<int>> SaveAsync(AddEditMaintenanceCommand request);

        Task<IResult<int>> DeleteAsync(int id);
        Task<IResult<GetAllMaintenancesResponse>> GetAsync(int id);

    }
}