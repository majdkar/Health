using SchoolV01.Application.Features.DeviceStatuss.Commands;
using SchoolV01.Application.Features.DeviceStatuss.Queries;
using SchoolV01.Application.Requests.DeviceStatuss;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public interface IDeviceStatusManager : IManager
    {
        Task<IResult<List<GetAllDeviceStatussResponse>>> GetAllAsync();
        
        Task<PaginatedResult<GetAllDeviceStatussResponse>> GetAllPagedAsync(GetAllPagedDeviceStatussRequest request);

        Task<IResult<int>> SaveAsync(AddEditDeviceStatusCommand request);

        Task<IResult<int>> DeleteAsync(int id);
        Task<IResult<GetAllDeviceStatussResponse>> GetAsync(int id);

    }
}