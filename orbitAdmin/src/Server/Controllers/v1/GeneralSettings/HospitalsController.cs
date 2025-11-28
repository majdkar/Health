using SchoolV01.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SchoolV01.Application.Features.Hospitals.Commands;
using SchoolV01.Application.Features.Hospitals.Queries;

namespace SchoolV01.Server.Controllers.v1.GeneralSettings
{
    public class HospitalsController : BaseApiController<HospitalsController>
    {

        /// <summary>
        /// Get All Paged Hospitals
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchString"></param>
        /// <param name="orderBy"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Hospitals.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString, string orderBy = null)
        {
            var Leavetypes = await Mediator.Send(new GetAllPagedHospitalsQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(Leavetypes);
        }



        /// <summary>
        /// Get All Paged Hospitals By DirectorateId
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
            var Leavetypes = await Mediator.Send(new GetAllPagedHospitalsByDirectorateIdQuery(DirectorateId, pageNumber, pageSize, searchString, orderBy));
            return Ok(Leavetypes);
        }




        /// <summary>
        /// Get All Hospitals
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Hospitals.View)]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var Hospitals = await Mediator.Send(new GetAllHospitalsQuery());
            return Ok(Hospitals);
        }

        /// <summary>
        /// Get a Hospital By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.Hospitals.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var Hospital = await Mediator.Send(new GetHospitalByIdQuery() { Id = id });
            return Ok(Hospital);
        }

        /// <summary>
        /// Create/Update a Hospital
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Hospitals.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditHospitalCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete a Hospital
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Hospitals.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteHospitalCommand { Id = id }));
        }

     
    }
}