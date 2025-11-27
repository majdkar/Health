using SchoolV01.Application.Features.Directorates.Commands;
using SchoolV01.Application.Features.Directorates.Queries;
using SchoolV01.Application.Requests.Directorates;
using SchoolV01.Client.Infrastructure.Extensions;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public class DirectorateManager : IDirectorateManager
    {
        private readonly HttpClient _httpClient;

        public DirectorateManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.DirectoratesEndpoints.Endpoints}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllDirectoratesResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.DirectoratesEndpoints.GetAll);
            return await response.ToResult<List<GetAllDirectoratesResponse>>();
        }

        public async Task<PaginatedResult<GetAllDirectoratesResponse>> GetAllPagedAsync(GetAllPagedDirectoratesRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.DirectoratesEndpoints.GetAllPaged(request.PageNumber,request.PageSize,request.SearchString,request.Orderby));
            return await response.ToPaginatedResult<GetAllDirectoratesResponse>();
        }

        public async Task<IResult<GetAllDirectoratesResponse>> GetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{Routes.DirectoratesEndpoints.Endpoints}/{id}");
            return await response.ToResult<GetAllDirectoratesResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditDirectorateCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.DirectoratesEndpoints.Endpoints, request);
            return await response.ToResult<int>();
        }
    }
}