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
    /// Controller responsible for managing dispenseactivityhistory related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting dispenseactivityhistory information.
    /// </remarks>
    [Route("api/dispenseactivityhistory")]
    [Authorize]
    public class DispenseActivityHistoryController : ControllerBase
    {
        private readonly IDispenseActivityHistoryService _dispenseActivityHistoryService;

        /// <summary>
        /// Initializes a new instance of the DispenseActivityHistoryController class with the specified context.
        /// </summary>
        /// <param name="idispenseactivityhistoryservice">The idispenseactivityhistoryservice to be used by the controller.</param>
        public DispenseActivityHistoryController(IDispenseActivityHistoryService idispenseactivityhistoryservice)
        {
            _dispenseActivityHistoryService = idispenseactivityhistoryservice;
        }

        /// <summary>Adds a new dispenseactivityhistory</summary>
        /// <param name="model">The dispenseactivityhistory data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("DispenseActivityHistory",Entitlements.Create)]
        public IActionResult Post([FromBody] DispenseActivityHistory model)
        {
            var id = _dispenseActivityHistoryService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of dispenseactivityhistorys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of dispenseactivityhistorys</returns>
        [HttpGet]
        [UserAuthorize("DispenseActivityHistory",Entitlements.Read)]
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

            var result = _dispenseActivityHistoryService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific dispenseactivityhistory by its primary key</summary>
        /// <param name="id">The primary key of the dispenseactivityhistory</param>
        /// <returns>The dispenseactivityhistory data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("DispenseActivityHistory",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _dispenseActivityHistoryService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific dispenseactivityhistory by its primary key</summary>
        /// <param name="id">The primary key of the dispenseactivityhistory</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("DispenseActivityHistory",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _dispenseActivityHistoryService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific dispenseactivityhistory by its primary key</summary>
        /// <param name="id">The primary key of the dispenseactivityhistory</param>
        /// <param name="updatedEntity">The dispenseactivityhistory data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("DispenseActivityHistory",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] DispenseActivityHistory updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _dispenseActivityHistoryService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific dispenseactivityhistory by its primary key</summary>
        /// <param name="id">The primary key of the dispenseactivityhistory</param>
        /// <param name="updatedEntity">The dispenseactivityhistory data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("DispenseActivityHistory",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<DispenseActivityHistory> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _dispenseActivityHistoryService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}