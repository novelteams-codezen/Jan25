using Microsoft.AspNetCore.Mvc;
using Jan25.Models;
using Jan25.Services;
using Jan25.Entities;
using Jan25.Filter;
using Jan25.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;

namespace Jan25.Controllers
{
    /// <summary>
    /// Controller responsible for managing patientcategory related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting patientcategory information.
    /// </remarks>
    [Route("api/patientcategory")]
    [Authorize]
    public class PatientCategoryController : ControllerBase
    {
        private readonly IPatientCategoryService _patientCategoryService;

        /// <summary>
        /// Initializes a new instance of the PatientCategoryController class with the specified context.
        /// </summary>
        /// <param name="ipatientcategoryservice">The ipatientcategoryservice to be used by the controller.</param>
        public PatientCategoryController(IPatientCategoryService ipatientcategoryservice)
        {
            _patientCategoryService = ipatientcategoryservice;
        }

        /// <summary>Adds a new patientcategory</summary>
        /// <param name="model">The patientcategory data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("PatientCategory",Entitlements.Create)]
        public IActionResult Post([FromBody] PatientCategory model)
        {
            var id = _patientCategoryService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of patientcategorys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of patientcategorys</returns>
        [HttpGet]
        [UserAuthorize("PatientCategory",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] string filters, string searchTerm, int pageNumber = 1, int pageSize = 10, string sortField = null, string sortOrder = "asc")
        {
            List<FilterCriteria> filterCriteria = null;
            if (pageSize < 1)
            {
                return BadRequest("Page size invalid.");
            }

            if (pageNumber < 1)
            {
                return BadRequest("Page mumber invalid.");
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var result = _patientCategoryService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific patientcategory by its primary key</summary>
        /// <param name="id">The primary key of the patientcategory</param>
        /// <returns>The patientcategory data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("PatientCategory",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _patientCategoryService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific patientcategory by its primary key</summary>
        /// <param name="id">The primary key of the patientcategory</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("PatientCategory",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _patientCategoryService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific patientcategory by its primary key</summary>
        /// <param name="id">The primary key of the patientcategory</param>
        /// <param name="updatedEntity">The patientcategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("PatientCategory",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] PatientCategory updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _patientCategoryService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific patientcategory by its primary key</summary>
        /// <param name="id">The primary key of the patientcategory</param>
        /// <param name="updatedEntity">The patientcategory data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("PatientCategory",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<PatientCategory> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _patientCategoryService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}