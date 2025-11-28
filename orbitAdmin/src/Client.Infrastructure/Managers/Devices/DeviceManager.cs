using SchoolV01.Application.Features.Devices.Commands;
using SchoolV01.Application.Features.Devices.Queries;
using SchoolV01.Application.Requests.Devices;
using SchoolV01.Client.Infrastructure.Extensions;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public class DeviceManager : IDeviceManager
    {
        private readonly HttpClient _httpClient;

        public DeviceManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.DevicesEndpoints.Endpoints}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllDevicesResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DevicesEndpoints.GetAll);
            return await response.ToResult<List<GetAllDevicesResponse>>();
        }

        public async Task<PaginatedResult<GetAllDevicesResponse>> GetAllPagedAsync(GetAllPagedDevicesRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.DevicesEndpoints.GetAllPaged(request.PageNumber,request.PageSize,request.SearchString,request.Orderby));
            return await response.ToPaginatedResult<GetAllDevicesResponse>();
        }

        public async Task<PaginatedResult<GetAllDevicesResponse>> GetAllPagedBySupplierIdAsync(GetAllPagedDevicesRequest request,int supplierId)
        {
            var response = await _httpClient.GetAsync(Routes.DevicesEndpoints.GetAllPagedBySupplier(request.PageNumber,request.PageSize,request.SearchString,request.Orderby,supplierId));
            return await response.ToPaginatedResult<GetAllDevicesResponse>();
        }

        public async Task<IResult<GetByIdDevicesResponse>> GetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{Routes.DevicesEndpoints.Endpoints}/{id}");
            return await response.ToResult<GetByIdDevicesResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditDeviceCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DevicesEndpoints.Endpoints, request);
            return await response.ToResult<int>();
        }
    }
}