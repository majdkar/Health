using SchoolV01.Application.Features.Maintenances.Commands;
using SchoolV01.Application.Features.Maintenances.Queries;
using SchoolV01.Application.Requests.Maintenances;
using SchoolV01.Client.Infrastructure.Extensions;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public class MaintenanceManager : IMaintenanceManager
    {
        private readonly HttpClient _httpClient;

        public MaintenanceManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.MaintenancesEndpoints.Endpoints}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllMaintenancesResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.MaintenancesEndpoints.GetAll);
            return await response.ToResult<List<GetAllMaintenancesResponse>>();
        }

        public async Task<PaginatedResult<GetAllMaintenancesResponse>> GetAllPagedAsync(GetAllPagedMaintenancesRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.MaintenancesEndpoints.GetAllPaged(request.PageNumber,request.PageSize,request.SearchString,request.Orderby,request.DeviceId));
            return await response.ToPaginatedResult<GetAllMaintenancesResponse>();
        }

        public async Task<IResult<GetAllMaintenancesResponse>> GetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{Routes.MaintenancesEndpoints.Endpoints}/{id}");
            return await response.ToResult<GetAllMaintenancesResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditMaintenanceCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.MaintenancesEndpoints.Endpoints, request);
            return await response.ToResult<int>();
        }
    }
}