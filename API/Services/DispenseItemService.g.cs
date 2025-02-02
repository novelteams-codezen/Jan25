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
    /// The dispenseitemService responsible for managing dispenseitem related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting dispenseitem information.
    /// </remarks>
    public interface IDispenseItemService
    {
        /// <summary>Retrieves a specific dispenseitem by its primary key</summary>
        /// <param name="id">The primary key of the dispenseitem</param>
        /// <returns>The dispenseitem data</returns>
        DispenseItem GetById(Guid id);

        /// <summary>Retrieves a list of dispenseitems based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of dispenseitems</returns>
        List<DispenseItem> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new dispenseitem</summary>
        /// <param name="model">The dispenseitem data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(DispenseItem model);

        /// <summary>Updates a specific dispenseitem by its primary key</summary>
        /// <param name="id">The primary key of the dispenseitem</param>
        /// <param name="updatedEntity">The dispenseitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, DispenseItem updatedEntity);

        /// <summary>Updates a specific dispenseitem by its primary key</summary>
        /// <param name="id">The primary key of the dispenseitem</param>
        /// <param name="updatedEntity">The dispenseitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<DispenseItem> updatedEntity);

        /// <summary>Deletes a specific dispenseitem by its primary key</summary>
        /// <param name="id">The primary key of the dispenseitem</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The dispenseitemService responsible for managing dispenseitem related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting dispenseitem information.
    /// </remarks>
    public class DispenseItemService : IDispenseItemService
    {
        private Jan25Context _dbContext;

        /// <summary>
        /// Initializes a new instance of the DispenseItem class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public DispenseItemService(Jan25Context dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific dispenseitem by its primary key</summary>
        /// <param name="id">The primary key of the dispenseitem</param>
        /// <returns>The dispenseitem data</returns>
        public DispenseItem GetById(Guid id)
        {
            var entityData = _dbContext.DispenseItem.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of dispenseitems based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of dispenseitems</returns>/// <exception cref="Exception"></exception>
        public List<DispenseItem> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetDispenseItem(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new dispenseitem</summary>
        /// <param name="model">The dispenseitem data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(DispenseItem model)
        {
            model.Id = CreateDispenseItem(model);
            return model.Id;
        }

        /// <summary>Updates a specific dispenseitem by its primary key</summary>
        /// <param name="id">The primary key of the dispenseitem</param>
        /// <param name="updatedEntity">The dispenseitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, DispenseItem updatedEntity)
        {
            UpdateDispenseItem(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific dispenseitem by its primary key</summary>
        /// <param name="id">The primary key of the dispenseitem</param>
        /// <param name="updatedEntity">The dispenseitem data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<DispenseItem> updatedEntity)
        {
            PatchDispenseItem(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific dispenseitem by its primary key</summary>
        /// <param name="id">The primary key of the dispenseitem</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteDispenseItem(id);
            return true;
        }
        #region
        private List<DispenseItem> GetDispenseItem(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.DispenseItem.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<DispenseItem>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(DispenseItem), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<DispenseItem, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateDispenseItem(DispenseItem model)
        {
            _dbContext.DispenseItem.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateDispenseItem(Guid id, DispenseItem updatedEntity)
        {
            _dbContext.DispenseItem.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteDispenseItem(Guid id)
        {
            var entityData = _dbContext.DispenseItem.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.DispenseItem.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchDispenseItem(Guid id, JsonPatchDocument<DispenseItem> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.DispenseItem.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.DispenseItem.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}