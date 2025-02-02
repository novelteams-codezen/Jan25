using Jan25.Models;
using Jan25.Data;
using Jan25.Filter;
using Jan25.Entities;
using Jan25.Logger;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;

namespace Jan25.Services
{
    /// <summary>
    /// The medicationcompositionService responsible for managing medicationcomposition related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting medicationcomposition information.
    /// </remarks>
    public interface IMedicationCompositionService
    {
        /// <summary>Retrieves a specific medicationcomposition by its primary key</summary>
        /// <param name="id">The primary key of the medicationcomposition</param>
        /// <returns>The medicationcomposition data</returns>
        MedicationComposition GetById(Guid id);

        /// <summary>Retrieves a list of medicationcompositions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of medicationcompositions</returns>
        List<MedicationComposition> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new medicationcomposition</summary>
        /// <param name="model">The medicationcomposition data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(MedicationComposition model);

        /// <summary>Updates a specific medicationcomposition by its primary key</summary>
        /// <param name="id">The primary key of the medicationcomposition</param>
        /// <param name="updatedEntity">The medicationcomposition data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, MedicationComposition updatedEntity);

        /// <summary>Updates a specific medicationcomposition by its primary key</summary>
        /// <param name="id">The primary key of the medicationcomposition</param>
        /// <param name="updatedEntity">The medicationcomposition data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<MedicationComposition> updatedEntity);

        /// <summary>Deletes a specific medicationcomposition by its primary key</summary>
        /// <param name="id">The primary key of the medicationcomposition</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The medicationcompositionService responsible for managing medicationcomposition related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting medicationcomposition information.
    /// </remarks>
    public class MedicationCompositionService : IMedicationCompositionService
    {
        private Jan25Context _dbContext;

        /// <summary>
        /// Initializes a new instance of the MedicationComposition class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public MedicationCompositionService(Jan25Context dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific medicationcomposition by its primary key</summary>
        /// <param name="id">The primary key of the medicationcomposition</param>
        /// <returns>The medicationcomposition data</returns>
        public MedicationComposition GetById(Guid id)
        {
            var entityData = _dbContext.MedicationComposition.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of medicationcompositions based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of medicationcompositions</returns>/// <exception cref="Exception"></exception>
        public List<MedicationComposition> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetMedicationComposition(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new medicationcomposition</summary>
        /// <param name="model">The medicationcomposition data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(MedicationComposition model)
        {
            model.Id = CreateMedicationComposition(model);
            return model.Id;
        }

        /// <summary>Updates a specific medicationcomposition by its primary key</summary>
        /// <param name="id">The primary key of the medicationcomposition</param>
        /// <param name="updatedEntity">The medicationcomposition data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, MedicationComposition updatedEntity)
        {
            UpdateMedicationComposition(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific medicationcomposition by its primary key</summary>
        /// <param name="id">The primary key of the medicationcomposition</param>
        /// <param name="updatedEntity">The medicationcomposition data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<MedicationComposition> updatedEntity)
        {
            PatchMedicationComposition(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific medicationcomposition by its primary key</summary>
        /// <param name="id">The primary key of the medicationcomposition</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteMedicationComposition(id);
            return true;
        }
        #region
        private List<MedicationComposition> GetMedicationComposition(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.MedicationComposition.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<MedicationComposition>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(MedicationComposition), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<MedicationComposition, object>>(Expression.Convert(property, typeof(object)), parameter);
                if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderBy(lambda);
                }
                else if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.OrderByDescending(lambda);
                }
                else
                {
                    throw new ApplicationException("Invalid sort order. Use 'asc' or 'desc'");
                }
            }

            var paginatedResult = result.Skip(skip).Take(pageSize).ToList();
            return paginatedResult;
        }

        private Guid CreateMedicationComposition(MedicationComposition model)
        {
            _dbContext.MedicationComposition.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateMedicationComposition(Guid id, MedicationComposition updatedEntity)
        {
            _dbContext.MedicationComposition.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteMedicationComposition(Guid id)
        {
            var entityData = _dbContext.MedicationComposition.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.MedicationComposition.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchMedicationComposition(Guid id, JsonPatchDocument<MedicationComposition> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.MedicationComposition.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.MedicationComposition.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}