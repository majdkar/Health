using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SchoolV01.Application.Interfaces.Repositories;
using SchoolV01.Application.Interfaces.Services.Identity;
using SchoolV01.Domain.Entities.GeneralSettings;
using SchoolV01.Domain.Enums;
using SchoolV01.Shared.Constants.Device;
using SchoolV01.Shared.Wrapper;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolV01.Application.Features.Dashboards.Queries.GetData
{
    public record GetDashboardInfoDataQuery() : IRequest<Result<DashboardInfoDataResponse>>;

    internal class GetDashboardInfoDataQueryHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<GetDashboardInfoDataQueryHandler> localizer)
        : IRequestHandler<GetDashboardInfoDataQuery, Result<DashboardInfoDataResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork = unitOfWork;
        private readonly IStringLocalizer<GetDashboardInfoDataQueryHandler> _localizer = localizer;

        public async Task<Result<DashboardInfoDataResponse>> Handle(GetDashboardInfoDataQuery query, CancellationToken cancellationToken)
        {
            var response = new DashboardInfoDataResponse
            {
                Clinic = await _unitOfWork.Repository<Clinic>().Entities.CountAsync(),
                  Supplier = await _unitOfWork.Repository<Supplier>().Entities.CountAsync(),
                Hospital = await _unitOfWork.Repository<Hospital>().Entities.CountAsync(),
                Directorate = await _unitOfWork.Repository<Directorate>().Entities.CountAsync(),
                DeviceCalibration = await _unitOfWork.Repository<Device>().Entities.Where(x => x.DeviceStatus == DeviceStatusEnum.DeviceNeedsCalibration.ToString()).CountAsync(),
                DeviceMaintenance = await _unitOfWork.Repository<Device>().Entities.Where(x => x.DeviceStatus == DeviceStatusEnum.DeviceNeedsMaintenance.ToString()).CountAsync(),
                DeviceWorked = await _unitOfWork.Repository<Device>().Entities.Where(x => x.DeviceStatus == DeviceStatusEnum.ItWorksWell.ToString()).CountAsync(),
                 DeviceCoordinator = await _unitOfWork.Repository<Device>().Entities.Where(x => x.DeviceStatus == DeviceStatusEnum.Coordinator.ToString()).CountAsync(),
                 DeviceNotWork = await _unitOfWork.Repository<Device>().Entities.Where(x => x.DeviceStatus == DeviceStatusEnum.ItdosenotWorks.ToString()).CountAsync(),

            };

            return await Result<DashboardInfoDataResponse>.SuccessAsync(response);
        }
    }
}