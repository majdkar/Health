using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SchoolV01.Application.Features.ProjectTypes.Commands;
using SchoolV01.Application.Features.ProjectTypes.Queries;

namespace SchoolV01.Server.Controllers.v1.GeneralSettings
{
    public class ProjectTypeController : BaseApiController<ProjectTypeController>
    {
        /// <summary>
        /// Get All ProjectTypes
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.ProjectTypes.View)]
        [HttpGet("GetAllLevels")]
        public async Task<IActionResult> GetAllLevels()
        {
            var Positions = await Mediator.Send(new GetAllLevelsProjectTypesQuery());
            return Ok(Positions);
        }

        /// <summary>
        /// Get All ProjectTypes
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.ProjectTypes.View)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Positions = await Mediator.Send(new GetAllProjectTypesQuery());
            return Ok(Positions);
        }

        /// <summary>
        /// Get a ProjectType By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.ProjectTypes.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Position = await Mediator.Send(new GetProjectTypeByIdQuery() { Id = id });
            return Ok(Position);
        }

        /// <summary>
        /// Create/Update a ProjectType
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.ProjectTypes.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditProjectTypeCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete a ProjectType
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.ProjectTypes.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteProjectTypeCommand { Id = id }));
        }

     
    }
}