using SchoolV01.Application.Features.Suppliers.Commands;
using SchoolV01.Application.Features.Suppliers.Queries;
using SchoolV01.Application.Requests.Suppliers;
using SchoolV01.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolV01.Client.Infrastructure.Managers.GeneralSettings
{
    public interface ISupplierManager : IManager
    {
        Task<IResult<List<GetAllSuppliersResponse>>> GetAllAsync();
        
        Task<PaginatedResult<GetAllSuppliersResponse>> GetAllPagedAsync(GetAllPagedSuppliersRequest request);

        Task<IResult<int>> SaveAsync(AddEditSupplierCommand request);

        Task<IResult<int>> DeleteAsync(int id);
        Task<IResult<GetAllSuppliersResponse>> GetAsync(int id);

    }
}