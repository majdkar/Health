using System;
using SchoolV01.Application.Features.Positions.Queries;
using SchoolV01.Client.Infrastructure.Extensions;
using SchoolV01.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SchoolV01.Application.Features.Positions.Commands;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public class PositionManager : IPositionManager
    {
        private readonly HttpClient _httpClient;

        public PositionManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.PositionsEndpoints.Endpoints}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllPositionsResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.PositionsEndpoints.GetAll);
            return await response.ToResult<List<GetAllPositionsResponse>>();
        }

        public async Task<IResult<GetAllPositionsResponse>> GetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{Routes.PositionsEndpoints.Endpoints}/{id}");
            return await response.ToResult<GetAllPositionsResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditPositionCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.PositionsEndpoints.Endpoints, request);
            return await response.ToResult<int>();
        }
    }
}