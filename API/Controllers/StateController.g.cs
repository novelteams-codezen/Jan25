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
    /// Controller responsible for managing state related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting state information.
    /// </remarks>
    [Route("api/state")]
    [Authorize]
    public class StateController : ControllerBase
    {
        private readonly IStateService _stateService;

        /// <summary>
        /// Initializes a new instance of the StateController class with the specified context.
        /// </summary>
        /// <param name="istateservice">The istateservice to be used by the controller.</param>
        public StateController(IStateService istateservice)
        {
            _stateService = istateservice;
        }

        /// <summary>Adds a new state</summary>
        /// <param name="model">The state data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("State",Entitlements.Create)]
        public IActionResult Post([FromBody] State model)
        {
            var id = _stateService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of states based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of states</returns>
        [HttpGet]
        [UserAuthorize("State",Entitlements.Read)]
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

            var result = _stateService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific state by its primary key</summary>
        /// <param name="id">The primary key of the state</param>
        /// <returns>The state data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("State",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _stateService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific state by its primary key</summary>
        /// <param name="id">The primary key of the state</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("State",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _stateService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific state by its primary key</summary>
        /// <param name="id">The primary key of the state</param>
        /// <param name="updatedEntity">The state data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("State",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] State updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _stateService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific state by its primary key</summary>
        /// <param name="id">The primary key of the state</param>
        /// <param name="updatedEntity">The state data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("State",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<State> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _stateService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}