using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SchoolV01.Application.Features.Devices.Commands;
using SchoolV01.Application.Features.Devices.Queries;

namespace SchoolV01.Server.Controllers.v1.GeneralSettings
{
    public class DevicesController : BaseApiController<DevicesController>
    {

        /// <summary>
        /// Get All Paged Devices
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Devices.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString, string orderBy = null)
        {
            var Leavetypes = await Mediator.Send(new GetAllPagedDevicesQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(Leavetypes);
        }

        /// <summary>
        /// Get All Paged Devices
        /// </summary>
        /// <param name="supplierId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Devices.View)]
        [HttpGet("BySupplierId")]
        public async Task<IActionResult> GetAllPagedBySupplierId( int supplierId ,int pageNumber, int pageSize, string searchString, string orderBy = null)
        {
            var Leavetypes = await Mediator.Send(new GetAllPagedDevicesBySupplierIdQuery( supplierId, pageNumber, pageSize, searchString, orderBy));
            return Ok(Leavetypes);
        }

        /// <summary>
        /// Get All Devices
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Devices.View)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Devices = await Mediator.Send(new GetAllDevicesQuery());
            return Ok(Devices);
        }

        /// <summary>
        /// Get a Device By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Devices.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Device = await Mediator.Send(new GetDeviceByIdQuery() { Id = id });
            return Ok(Device);
        }

        /// <summary>
        /// Create/Update a Device
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Devices.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditDeviceCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete a Device
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Devices.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteDeviceCommand { Id = id }));
        }

     
    }
}