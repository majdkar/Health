using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SchoolV01.Application.Features.Directorates.Commands;
using SchoolV01.Application.Features.Directorates.Queries;

namespace SchoolV01.Server.Controllers.v1.GeneralSettings
{
    public class DirectoratesController : BaseApiController<DirectoratesController>
    {

        /// <summary>
        /// Get All Paged Directorates
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Directorates.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString, string orderBy = null)
        {
            var Leavetypes = await Mediator.Send(new GetAllPagedDirectoratesQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(Leavetypes);
        }

        /// <summary>
        /// Get All Directorates
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Directorates.View)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Directorates = await Mediator.Send(new GetAllDirectoratesQuery());
            return Ok(Directorates);
        }

        /// <summary>
        /// Get a Directorate By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Directorates.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Directorate = await Mediator.Send(new GetDirectorateByIdQuery() { Id = id });
            return Ok(Directorate);
        }

        /// <summary>
        /// Create/Update a Directorate
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Directorates.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditDirectorateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete a Directorate
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Directorates.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteDirectorateCommand { Id = id }));
        }

     
    }
}