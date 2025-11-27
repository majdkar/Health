using SchoolV01.Application.Features.Suppliers.Commands;
using SchoolV01.Application.Features.Suppliers.Queries;
using SchoolV01.Application.Requests.Suppliers;
using SchoolV01.Client.Infrastructure.Extensions;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public class SupplierManager : ISupplierManager
    {
        private readonly HttpClient _httpClient;

        public SupplierManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<IResult<int>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.SuppliersEndpoints.Endpoints}/{id}");
            return await response.ToResult<int>();
        }

        public async Task<IResult<List<GetAllSuppliersResponse>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync(Routes.SuppliersEndpoints.GetAll);
            return await response.ToResult<List<GetAllSuppliersResponse>>();
        }

        public async Task<PaginatedResult<GetAllSuppliersResponse>> GetAllPagedAsync(GetAllPagedSuppliersRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.SuppliersEndpoints.GetAllPaged(request.PageNumber,request.PageSize,request.SearchString,request.Orderby));
            return await response.ToPaginatedResult<GetAllSuppliersResponse>();
        }

        public async Task<IResult<GetAllSuppliersResponse>> GetAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{Routes.SuppliersEndpoints.Endpoints}/{id}");
            return await response.ToResult<GetAllSuppliersResponse>();
        }

        public async Task<IResult<int>> SaveAsync(AddEditSupplierCommand request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.SuppliersEndpoints.Endpoints, request);
            return await response.ToResult<int>();
        }
    }
}