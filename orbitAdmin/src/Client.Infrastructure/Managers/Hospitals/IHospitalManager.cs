using SchoolV01.Application.Features.Hospitals.Commands;
using SchoolV01.Application.Features.Hospitals.Queries;
using SchoolV01.Application.Requests.Hospitals;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public interface IHospitalManager : IManager
    {
        Task<IResult<List<GetAllHospitalsResponse>>> GetAllAsync();
        
        Task<PaginatedResult<GetAllHospitalsResponse>> GetAllPagedAsync(GetAllPagedHospitalsRequest request);
        Task<PaginatedResult<GetAllHospitalsResponse>> GetAllPagedByDirectorateIdAsync(GetAllPagedHospitalsRequest request,int DirectorateId);

        Task<IResult<int>> SaveAsync(AddEditHospitalCommand request);

        Task<IResult<int>> DeleteAsync(int id);
        Task<IResult<GetAllHospitalsResponse>> GetAsync(int id);

    }
}