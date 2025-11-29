using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SchoolV01.Application.Features.DeviceStatuss.Commands;
using SchoolV01.Application.Features.DeviceStatuss.Queries;

namespace SchoolV01.Server.Controllers.v1.GeneralSettings
{
    public class DeviceStatussController : BaseApiController<DeviceStatussController>
    {

        /// <summary>
        /// Get All Paged DeviceStatuss
        /// </summary>
        /// <param name="DeviceId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.DeviceStatuss.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int DeviceId ,int pageNumber, int pageSize, string searchString, string orderBy = null)
        {
            var Leavetypes = await Mediator.Send(new GetAllPagedDeviceStatussQuery(DeviceId, pageNumber, pageSize, searchString, orderBy));
            return Ok(Leavetypes);
        }

        /// <summary>
        /// Get All DeviceStatuss
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.DeviceStatuss.View)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var DeviceStatuss = await Mediator.Send(new GetAllDeviceStatussQuery());
            return Ok(DeviceStatuss);
        }

        /// <summary>
        /// Get a DeviceStatus By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.DeviceStatuss.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var DeviceStatus = await Mediator.Send(new GetDeviceStatusByIdQuery() { Id = id });
            return Ok(DeviceStatus);
        }

        /// <summary>
        /// Create/Update a DeviceStatus
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.DeviceStatuss.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditDeviceStatusCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete a DeviceStatus
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.DeviceStatuss.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteDeviceStatusCommand { Id = id }));
        }

     
    }
}