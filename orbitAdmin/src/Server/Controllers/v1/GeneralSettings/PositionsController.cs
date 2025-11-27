using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SchoolV01.Application.Features.Positions.Commands;
using SchoolV01.Application.Features.Positions.Queries;

namespace SchoolV01.Server.Controllers.v1.GeneralSettings
{
    public class PositionsController : BaseApiController<PositionsController>
    {
        /// <summary>
        /// Get All Positions
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Positions.View)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Positions = await Mediator.Send(new GetAllPositionsQuery());
            return Ok(Positions);
        }

        /// <summary>
        /// Get a Position By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Positions.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Position = await Mediator.Send(new GetPositionByIdQuery() { Id = id });
            return Ok(Position);
        }

        /// <summary>
        /// Create/Update a Position
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Positions.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditPositionCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete a Position
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Positions.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeletePositionCommand { Id = id }));
        }

     
    }
}