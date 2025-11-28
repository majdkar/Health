using SchoolV01.Application.Features.Clinics.Commands;
using SchoolV01.Application.Features.Clinics.Queries;
using SchoolV01.Application.Requests.Clinics;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public interface IClinicManager : IManager
    {
        Task<IResult<List<GetAllClinicsResponse>>> GetAllAsync();
        
        Task<PaginatedResult<GetAllClinicsResponse>> GetAllPagedAsync(GetAllPagedClinicsRequest request);
        Task<PaginatedResult<GetAllClinicsResponse>> GetAllPagedByDirectorateIdAsync(GetAllPagedClinicsRequest request,int supplierId);

        Task<IResult<int>> SaveAsync(AddEditClinicCommand request);

        Task<IResult<int>> DeleteAsync(int id);
        Task<IResult<GetAllClinicsResponse>> GetAsync(int id);

    }
}