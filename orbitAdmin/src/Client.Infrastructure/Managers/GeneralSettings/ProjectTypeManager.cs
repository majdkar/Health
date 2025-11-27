using System;
using SchoolV01.Application.Features.ProjectTypes.Queries;
using SchoolV01.Client.Infrastructure.Extensions;
using SchoolV01.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SchoolV01.Application.Features.ProjectTypes.Commands;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public class ProjectTypeManager : IProjectTypeManager
    {
        private readonly HttpClient _httpClient;

        public ProjectTypeManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.ProjectTypeEndpoints.Endpoints}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllProjectTypesResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.ProjectTypeEndpoints.GetAll);
            return await response.ToResult<List<GetAllProjectTypesResponse>>();
        }
        

        public async Task<IResult<List<GetAllProjectTypesResponse>>> GetAllLevelsAsync()
        {
            var response = await _httpClient.GetAsync(Routes.ProjectTypeEndpoints.GetAllLevels);
            return await response.ToResult<List<GetAllProjectTypesResponse>>();
        }

        public async Task<IResult<GetAllProjectTypesResponse>> GetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{Routes.ProjectTypeEndpoints.Endpoints}/{id}");
            return await response.ToResult<GetAllProjectTypesResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditProjectTypeCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.ProjectTypeEndpoints.Endpoints, request);
            return await response.ToResult<int>();
        }
    }
}