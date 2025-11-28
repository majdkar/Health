using SchoolV01.Application.Features.Reports;

using SchoolV01.Application.Requests.Reports;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public interface IReportManager : IManager
    {
        
        Task<PaginatedResult<GetAllDeviceReportsResponse>> GetAllPagedReportDeviceAsync(GetAllPagedReportsRequest request);


    }
}