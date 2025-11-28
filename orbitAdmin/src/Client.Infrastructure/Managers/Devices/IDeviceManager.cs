using SchoolV01.Application.Features.Devices.Commands;
using SchoolV01.Application.Features.Devices.Queries;
using SchoolV01.Application.Requests.Devices;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public interface IDeviceManager : IManager
    {
        Task<IResult<List<GetAllDevicesResponse>>> GetAllAsync();
        
        Task<PaginatedResult<GetAllDevicesResponse>> GetAllPagedAsync(GetAllPagedDevicesRequest request);
        Task<PaginatedResult<GetAllDevicesResponse>> GetAllPagedBySupplierIdAsync(GetAllPagedDevicesRequest request,int supplierId);

        Task<IResult<int>> SaveAsync(AddEditDeviceCommand request);

        Task<IResult<int>> DeleteAsync(int id);
        Task<IResult<GetByIdDevicesResponse>> GetAsync(int id);

    }
}