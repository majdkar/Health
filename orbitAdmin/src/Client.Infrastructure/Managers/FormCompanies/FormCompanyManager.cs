using SchoolV01.Application.Features.FormCompanys.Commands;
using SchoolV01.Application.Features.FormCompanies.Queries;
using SchoolV01.Application.Requests.FormCompanies;
using SchoolV01.Client.Infrastructure.Extensions;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public class FormCompanyManager : IFormCompanyManager
    {
        private readonly HttpClient _httpClient;

        public FormCompanyManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.FormCompanysEndpoints.Endpoints}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<PaginatedResult<GetAllFormCompaniesResponse>> GetAllPagedAsync(GetAllPagedFormCompaniesRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.FormCompanysEndpoints.GetAllPaged(request.PageNumber,request.PageSize,request.SearchString,request.Orderby));
            return await response.ToPaginatedResult<GetAllFormCompaniesResponse>();
        }

      

        public async Task<IResult<GetByIdFormCompaniesResponse>> GetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{Routes.FormCompanysEndpoints.Endpoints}/{id}");
            return await response.ToResult<GetByIdFormCompaniesResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditFormCompanyCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.FormCompanysEndpoints.Endpoints, request);
            return await response.ToResult<int>();
        }
    }
}