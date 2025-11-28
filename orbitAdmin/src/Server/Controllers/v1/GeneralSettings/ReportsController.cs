using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolV01.Application.Features.Devices.Commands;
using SchoolV01.Application.Features.Devices.Queries;
using SchoolV01.Application.Features.Reports;
using SchoolV01.Application.Requests.Reports;
using SchoolV01.Shared.Constants.Permission;
using System.Threading.Tasks;

namespace SchoolV01.Server.Controllers.v1.GeneralSettings
{
    public class ReportsController : BaseApiController<ReportsController>
    {
        /// <summary>
        /// Get All Paged Devices
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Devices.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllPagedReportsRequest request)
        {
            var result = await Mediator.Send(new GetAllPagedDeviceReportsQuery(
                request.PageNumber,
                request.PageSize,
                request.SearchString,
                request.DeviceNameAr,
                request.DeviceNameEn,
                request.DeviceStatus,
                request.ProjectTypeId,
                request.SubProjectTypeId,
                request.CityId,
                request.ClinicId,
                request.HospitalId,
                request.DirectorateId,
                request.Year,
                request.SerialNumber,
                request.RunFrom,
                request.RunTo,
                request.Orderby
            ));

            return Ok(result);
        }



    }
}