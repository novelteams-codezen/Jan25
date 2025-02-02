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
    /// The contactmemberService responsible for managing contactmember related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting contactmember information.
    /// </remarks>
    public interface IContactMemberService
    {
        /// <summary>Retrieves a specific contactmember by its primary key</summary>
        /// <param name="id">The primary key of the contactmember</param>
        /// <returns>The contactmember data</returns>
        ContactMember GetById(Guid id);

        /// <summary>Retrieves a list of contactmembers based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of contactmembers</returns>
        List<ContactMember> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new contactmember</summary>
        /// <param name="model">The contactmember data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(ContactMember model);

        /// <summary>Updates a specific contactmember by its primary key</summary>
        /// <param name="id">The primary key of the contactmember</param>
        /// <param name="updatedEntity">The contactmember data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, ContactMember updatedEntity);

        /// <summary>Updates a specific contactmember by its primary key</summary>
        /// <param name="id">The primary key of the contactmember</param>
        /// <param name="updatedEntity">The contactmember data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<ContactMember> updatedEntity);

        /// <summary>Deletes a specific contactmember by its primary key</summary>
        /// <param name="id">The primary key of the contactmember</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The contactmemberService responsible for managing contactmember related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting contactmember information.
    /// </remarks>
    public class ContactMemberService : IContactMemberService
    {
        private Jan25Context _dbContext;

        /// <summary>
        /// Initializes a new instance of the ContactMember class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public ContactMemberService(Jan25Context dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific contactmember by its primary key</summary>
        /// <param name="id">The primary key of the contactmember</param>
        /// <returns>The contactmember data</returns>
        public ContactMember GetById(Guid id)
        {
            var entityData = _dbContext.ContactMember.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of contactmembers based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of contactmembers</returns>/// <exception cref="Exception"></exception>
        public List<ContactMember> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetContactMember(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new contactmember</summary>
        /// <param name="model">The contactmember data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(ContactMember model)
        {
            model.Id = CreateContactMember(model);
            return model.Id;
        }

        /// <summary>Updates a specific contactmember by its primary key</summary>
        /// <param name="id">The primary key of the contactmember</param>
        /// <param name="updatedEntity">The contactmember data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, ContactMember updatedEntity)
        {
            UpdateContactMember(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific contactmember by its primary key</summary>
        /// <param name="id">The primary key of the contactmember</param>
        /// <param name="updatedEntity">The contactmember data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<ContactMember> updatedEntity)
        {
            PatchContactMember(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific contactmember by its primary key</summary>
        /// <param name="id">The primary key of the contactmember</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteContactMember(id);
            return true;
        }
        #region
        private List<ContactMember> GetContactMember(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.ContactMember.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<ContactMember>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(ContactMember), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<ContactMember, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateContactMember(ContactMember model)
        {
            _dbContext.ContactMember.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateContactMember(Guid id, ContactMember updatedEntity)
        {
            _dbContext.ContactMember.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteContactMember(Guid id)
        {
            var entityData = _dbContext.ContactMember.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.ContactMember.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchContactMember(Guid id, JsonPatchDocument<ContactMember> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.ContactMember.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.ContactMember.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}