using System;
using SchoolV01.Application.Features.ProjectTypes.Queries;
using SchoolV01.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolV01.Application.Features.ProjectTypes.Commands;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public interface IProjectTypeManager : IManager
    {
        Task<IResult<List<GetAllProjectTypesResponse>>> GetAllAsync();
        Task<IResult<List<GetAllProjectTypesResponse>>> GetAllLevelsAsync();

        Task<IResult<int>> SaveAsync(AddEditProjectTypeCommand request);

        Task<IResult<int>> DeleteAsync(int id);
        Task<IResult<GetAllProjectTypesResponse>> GetAsync(int id);

    }
}