using System;
using SchoolV01.Application.Features.Positions.Queries;
using SchoolV01.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using SchoolV01.Application.Features.Positions.Commands;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public interface IPositionManager : IManager
    {
        Task<IResult<List<GetAllPositionsResponse>>> GetAllAsync();

        Task<IResult<int>> SaveAsync(AddEditPositionCommand request);

        Task<IResult<int>> DeleteAsync(int id);
        Task<IResult<GetAllPositionsResponse>> GetAsync(int id);

    }
}