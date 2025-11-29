using SchoolV01.Application.Features.DeviceStatuss.Commands;
using SchoolV01.Application.Features.DeviceStatuss.Queries;
using SchoolV01.Application.Requests.DeviceStatuss;
using SchoolV01.Client.Infrastructure.Extensions;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public class DeviceStatusManager : IDeviceStatusManager
    {
        private readonly HttpClient _httpClient;

        public DeviceStatusManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.DeviceStatussEndpoints.Endpoints}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllDeviceStatussResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DeviceStatussEndpoints.GetAll);
            return await response.ToResult<List<GetAllDeviceStatussResponse>>();
        }

        public async Task<PaginatedResult<GetAllDeviceStatussResponse>> GetAllPagedAsync(GetAllPagedDeviceStatussRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.DeviceStatussEndpoints.GetAllPaged(request.PageNumber,request.PageSize,request.SearchString,request.Orderby,request.DeviceId));
            return await response.ToPaginatedResult<GetAllDeviceStatussResponse>();
        }

        public async Task<IResult<GetAllDeviceStatussResponse>> GetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{Routes.DeviceStatussEndpoints.Endpoints}/{id}");
            return await response.ToResult<GetAllDeviceStatussResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditDeviceStatusCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DeviceStatussEndpoints.Endpoints, request);
            return await response.ToResult<int>();
        }
    }
}