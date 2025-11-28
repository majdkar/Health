using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolV01.Application.Features.Clinics.Commands;
using SchoolV01.Application.Features.Clinics.Queries;
using SchoolV01.Application.Features.Hospitals.Queries;
using SchoolV01.Shared.Constants.Permission;
using System.Threading.Tasks;

namespace SchoolV01.Server.Controllers.v1.GeneralSettings
{
    public class ClinicsController : BaseApiController<ClinicsController>
    {

        /// <summary>
        /// Get All Paged Clinics
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Clinics.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString, string orderBy = null)
        {
            var Leavetypes = await Mediator.Send(new GetAllPagedClinicsQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(Leavetypes);
        }




        /// <summary>
        /// Get All Paged Clinics By DirectorateId
        /// </summary>
        /// <param name="DirectorateId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Hospitals.View)]
        [HttpGet("ByDirectorateId")]
        public async Task<IActionResult> GetAllPagedByDirectorateId(int DirectorateId, int pageNumber, int pageSize, string searchString, string orderBy = null)
        {
            var Leavetypes = await Mediator.Send(new GetAllPagedClinicsByDirectorateIdQuery(DirectorateId, pageNumber, pageSize, searchString, orderBy));
            return Ok(Leavetypes);
        }


        /// <summary>
        /// Get All Clinics
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Clinics.View)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Clinics = await Mediator.Send(new GetAllClinicsQuery());
            return Ok(Clinics);
        }

        /// <summary>
        /// Get a Clinic By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Clinics.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Clinic = await Mediator.Send(new GetClinicByIdQuery() { Id = id });
            return Ok(Clinic);
        }

        /// <summary>
        /// Create/Update a Clinic
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Clinics.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditClinicCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete a Clinic
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Clinics.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteClinicCommand { Id = id }));
        }

     
    }
}