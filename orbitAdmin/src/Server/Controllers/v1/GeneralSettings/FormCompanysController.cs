using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SchoolV01.Application.Features.FormCompanys.Commands;
using SchoolV01.Application.Features.FormCompanies.Queries;

namespace SchoolV01.Server.Controllers.v1.GeneralSettings
{
    public class FormCompanysController : BaseApiController<FormCompanysController>
    {

        /// <summary>
        /// Get All Paged Forms
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Forms.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString, string orderBy = null)
        {
            var Leavetypes = await Mediator.Send(new GetAllPagedFormCompaniesQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(Leavetypes);
        }


        /// <summary>
        /// Get a FormCompany By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Forms.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var FormCompany = await Mediator.Send(new GetFormCompanyByIdQuery() { Id = id });
            return Ok(FormCompany);
        }

        /// <summary>
        /// Create/Update a FormCompany
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditFormCompanyCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete a FormCompany
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Forms.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteFormCompanyCommand { Id = id }));
        }

     
    }
}