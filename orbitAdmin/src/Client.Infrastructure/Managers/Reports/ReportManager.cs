
using SchoolV01.Application.Features.Reports;
using SchoolV01.Application.Requests.Reports;
using SchoolV01.Client.Infrastructure.Extensions;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public class ReportManager : IReportManager
    {
        private readonly HttpClient _httpClient;

        public ReportManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PaginatedResult<GetAllDeviceReportsResponse>> GetAllPagedReportDeviceAsync(GetAllPagedReportsRequest request)
        {
            var response = await _httpClient.GetAsync(Routes.ReportsEndpoints.GetAllPaged(request));
            return await response.ToPaginatedResult<GetAllDeviceReportsResponse>();
        }

    }
}