using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SchoolV01.Application.Features.Suppliers.Commands;
using SchoolV01.Application.Features.Suppliers.Queries;

namespace SchoolV01.Server.Controllers.v1.GeneralSettings
{
    public class SuppliersController : BaseApiController<SuppliersController>
    {

        /// <summary>
        /// Get All Paged Suppliers
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Suppliers.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString, string orderBy = null)
        {
            var Leavetypes = await Mediator.Send(new GetAllPagedSuppliersQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(Leavetypes);
        }

        /// <summary>
        /// Get All Suppliers
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Suppliers.View)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Suppliers = await Mediator.Send(new GetAllSuppliersQuery());
            return Ok(Suppliers);
        }

        /// <summary>
        /// Get a Supplier By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Suppliers.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Supplier = await Mediator.Send(new GetSupplierByIdQuery() { Id = id });
            return Ok(Supplier);
        }

        /// <summary>
        /// Create/Update a Supplier
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Suppliers.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditSupplierCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete a Supplier
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Suppliers.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteSupplierCommand { Id = id }));
        }

     
    }
}