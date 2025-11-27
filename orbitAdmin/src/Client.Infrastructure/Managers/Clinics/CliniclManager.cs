using SchoolV01.Application.Features.Clinics.Commands;
using SchoolV01.Application.Features.Clinics.Queries;
using SchoolV01.Application.Requests.Clinics;
using SchoolV01.Client.Infrastructure.Extensions;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public class ClinicManager : IClinicManager
    {
        private readonly HttpClient _httpClient;

        public ClinicManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.ClinicsEndpoints.Endpoints}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllClinicsResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.ClinicsEndpoints.GetAll);
            return await response.ToResult<List<GetAllClinicsResponse>>();
        }

        public async Task<PaginatedResult<GetAllClinicsResponse>> GetAllPagedAsync(GetAllPagedClinicsRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.ClinicsEndpoints.GetAllPaged(request.PageNumber,request.PageSize,request.SearchString,request.Orderby));
            return await response.ToPaginatedResult<GetAllClinicsResponse>();
        }

        public async Task<IResult<GetAllClinicsResponse>> GetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{Routes.ClinicsEndpoints.Endpoints}/{id}");
            return await response.ToResult<GetAllClinicsResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditClinicCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.ClinicsEndpoints.Endpoints, request);
            return await response.ToResult<int>();
        }
    }
}