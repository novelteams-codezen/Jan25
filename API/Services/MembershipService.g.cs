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
    /// The membershipService responsible for managing membership related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting membership information.
    /// </remarks>
    public interface IMembershipService
    {
        /// <summary>Retrieves a specific membership by its primary key</summary>
        /// <param name="id">The primary key of the membership</param>
        /// <returns>The membership data</returns>
        Membership GetById(Guid id);

        /// <summary>Retrieves a list of memberships based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of memberships</returns>
        List<Membership> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new membership</summary>
        /// <param name="model">The membership data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Membership model);

        /// <summary>Updates a specific membership by its primary key</summary>
        /// <param name="id">The primary key of the membership</param>
        /// <param name="updatedEntity">The membership data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Membership updatedEntity);

        /// <summary>Updates a specific membership by its primary key</summary>
        /// <param name="id">The primary key of the membership</param>
        /// <param name="updatedEntity">The membership data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Membership> updatedEntity);

        /// <summary>Deletes a specific membership by its primary key</summary>
        /// <param name="id">The primary key of the membership</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The membershipService responsible for managing membership related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting membership information.
    /// </remarks>
    public class MembershipService : IMembershipService
    {
        private Jan25Context _dbContext;

        /// <summary>
        /// Initializes a new instance of the Membership class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public MembershipService(Jan25Context dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific membership by its primary key</summary>
        /// <param name="id">The primary key of the membership</param>
        /// <returns>The membership data</returns>
        public Membership GetById(Guid id)
        {
            var entityData = _dbContext.Membership.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of memberships based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of memberships</returns>/// <exception cref="Exception"></exception>
        public List<Membership> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetMembership(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new membership</summary>
        /// <param name="model">The membership data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Membership model)
        {
            model.Id = CreateMembership(model);
            return model.Id;
        }

        /// <summary>Updates a specific membership by its primary key</summary>
        /// <param name="id">The primary key of the membership</param>
        /// <param name="updatedEntity">The membership data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Membership updatedEntity)
        {
            UpdateMembership(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific membership by its primary key</summary>
        /// <param name="id">The primary key of the membership</param>
        /// <param name="updatedEntity">The membership data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Membership> updatedEntity)
        {
            PatchMembership(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific membership by its primary key</summary>
        /// <param name="id">The primary key of the membership</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteMembership(id);
            return true;
        }
        #region
        private List<Membership> GetMembership(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Membership.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Membership>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Membership), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Membership, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateMembership(Membership model)
        {
            _dbContext.Membership.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateMembership(Guid id, Membership updatedEntity)
        {
            _dbContext.Membership.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteMembership(Guid id)
        {
            var entityData = _dbContext.Membership.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Membership.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchMembership(Guid id, JsonPatchDocument<Membership> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Membership.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Membership.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}