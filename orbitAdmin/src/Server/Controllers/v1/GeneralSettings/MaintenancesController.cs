using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SchoolV01.Application.Features.Maintenances.Commands;
using SchoolV01.Application.Features.Maintenances.Queries;

namespace SchoolV01.Server.Controllers.v1.GeneralSettings
{
    public class MaintenancesController : BaseApiController<MaintenancesController>
    {

        /// <summary>
        /// Get All Paged Maintenances
        /// </summary>
        /// <param name="DeviceId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Maintenances.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int DeviceId ,int pageNumber, int pageSize, string searchString, string orderBy = null)
        {
            var Leavetypes = await Mediator.Send(new GetAllPagedMaintenancesQuery(DeviceId, pageNumber, pageSize, searchString, orderBy));
            return Ok(Leavetypes);
        }

        /// <summary>
        /// Get All Maintenances
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Maintenances.View)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Maintenances = await Mediator.Send(new GetAllMaintenancesQuery());
            return Ok(Maintenances);
        }

        /// <summary>
        /// Get a Maintenance By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Maintenances.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Maintenance = await Mediator.Send(new GetMaintenanceByIdQuery() { Id = id });
            return Ok(Maintenance);
        }

        /// <summary>
        /// Create/Update a Maintenance
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Maintenances.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditMaintenanceCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete a Maintenance
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Maintenances.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteMaintenanceCommand { Id = id }));
        }

     
    }
}