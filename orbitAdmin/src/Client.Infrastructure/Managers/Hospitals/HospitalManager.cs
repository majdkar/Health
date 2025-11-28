using SchoolV01.Application.Features.Hospitals.Commands;
using SchoolV01.Application.Features.Hospitals.Queries;
using SchoolV01.Application.Requests.Hospitals;
using SchoolV01.Client.Infrastructure.Extensions;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public class HospitalManager : IHospitalManager
    {
        private readonly HttpClient _httpClient;

        public HospitalManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.HospitalsEndpoints.Endpoints}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllHospitalsResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.HospitalsEndpoints.GetAll);
            return await response.ToResult<List<GetAllHospitalsResponse>>();
        }

        public async Task<PaginatedResult<GetAllHospitalsResponse>> GetAllPagedAsync(GetAllPagedHospitalsRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.HospitalsEndpoints.GetAllPaged(request.PageNumber,request.PageSize,request.SearchString,request.Orderby));
            return await response.ToPaginatedResult<GetAllHospitalsResponse>();
        }

        public async Task<PaginatedResult<GetAllHospitalsResponse>> GetAllPagedByDirectorateIdAsync(GetAllPagedHospitalsRequest request,int supplierId)
        {
            var response = await _httpClient.GetAsync(Routes.HospitalsEndpoints.GetAllPagedByDirectorateId(supplierId, request.PageNumber,request.PageSize,request.SearchString,request.Orderby));
            return await response.ToPaginatedResult<GetAllHospitalsResponse>();
        }

        public async Task<IResult<GetAllHospitalsResponse>> GetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{Routes.HospitalsEndpoints.Endpoints}/{id}");
            return await response.ToResult<GetAllHospitalsResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditHospitalCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.HospitalsEndpoints.Endpoints, request);
            return await response.ToResult<int>();
        }
    }
}