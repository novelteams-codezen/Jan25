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
    /// The countryService responsible for managing country related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting country information.
    /// </remarks>
    public interface ICountryService
    {
        /// <summary>Retrieves a specific country by its primary key</summary>
        /// <param name="id">The primary key of the country</param>
        /// <returns>The country data</returns>
        Country GetById(Guid id);

        /// <summary>Retrieves a list of countrys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of countrys</returns>
        List<Country> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc");

        /// <summary>Adds a new country</summary>
        /// <param name="model">The country data to be added</param>
        /// <returns>The result of the operation</returns>
        Guid Create(Country model);

        /// <summary>Updates a specific country by its primary key</summary>
        /// <param name="id">The primary key of the country</param>
        /// <param name="updatedEntity">The country data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Update(Guid id, Country updatedEntity);

        /// <summary>Updates a specific country by its primary key</summary>
        /// <param name="id">The primary key of the country</param>
        /// <param name="updatedEntity">The country data to be updated</param>
        /// <returns>The result of the operation</returns>
        bool Patch(Guid id, JsonPatchDocument<Country> updatedEntity);

        /// <summary>Deletes a specific country by its primary key</summary>
        /// <param name="id">The primary key of the country</param>
        /// <returns>The result of the operation</returns>
        bool Delete(Guid id);
    }

    /// <summary>
    /// The countryService responsible for managing country related operations.
    /// </summary>
    /// <remarks>
    /// This service for adding, retrieving, updating, and deleting country information.
    /// </remarks>
    public class CountryService : ICountryService
    {
        private Jan25Context _dbContext;

        /// <summary>
        /// Initializes a new instance of the Country class.
        /// </summary>
        /// <param name="dbContext">dbContext value to set.</param>
        public CountryService(Jan25Context dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>Retrieves a specific country by its primary key</summary>
        /// <param name="id">The primary key of the country</param>
        /// <returns>The country data</returns>
        public Country GetById(Guid id)
        {
            var entityData = _dbContext.Country.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            return entityData;
        }

        /// <summary>Retrieves a list of countrys based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of countrys</returns>/// <exception cref="Exception"></exception>
        public List<Country> Get(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            var result = GetCountry(filters, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return result;
        }

        /// <summary>Adds a new country</summary>
        /// <param name="model">The country data to be added</param>
        /// <returns>The result of the operation</returns>
        public Guid Create(Country model)
        {
            model.Id = CreateCountry(model);
            return model.Id;
        }

        /// <summary>Updates a specific country by its primary key</summary>
        /// <param name="id">The primary key of the country</param>
        /// <param name="updatedEntity">The country data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Update(Guid id, Country updatedEntity)
        {
            UpdateCountry(id, updatedEntity);
            return true;
        }

        /// <summary>Updates a specific country by its primary key</summary>
        /// <param name="id">The primary key of the country</param>
        /// <param name="updatedEntity">The country data to be updated</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Patch(Guid id, JsonPatchDocument<Country> updatedEntity)
        {
            PatchCountry(id, updatedEntity);
            return true;
        }

        /// <summary>Deletes a specific country by its primary key</summary>
        /// <param name="id">The primary key of the country</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception"></exception>
        public bool Delete(Guid id)
        {
            DeleteCountry(id);
            return true;
        }
        #region
        private List<Country> GetCountry(List<FilterCriteria> filters = null, string searchTerm = "", int pageNumber = 1, int pageSize = 1, string sortField = null, string sortOrder = "asc")
        {
            if (pageSize < 1)
            {
                throw new ApplicationException("Page size invalid!");
            }

            if (pageNumber < 1)
            {
                throw new ApplicationException("Page mumber invalid!");
            }

            var query = _dbContext.Country.IncludeRelated().AsQueryable();
            int skip = (pageNumber - 1) * pageSize;
            var result = FilterService<Country>.ApplyFilter(query, filters, searchTerm);
            if (!string.IsNullOrEmpty(sortField))
            {
                var parameter = Expression.Parameter(typeof(Country), "b");
                var property = Expression.Property(parameter, sortField);
                var lambda = Expression.Lambda<Func<Country, object>>(Expression.Convert(property, typeof(object)), parameter);
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

        private Guid CreateCountry(Country model)
        {
            _dbContext.Country.Add(model);
            _dbContext.SaveChanges();
            return model.Id;
        }

        private void UpdateCountry(Guid id, Country updatedEntity)
        {
            _dbContext.Country.Update(updatedEntity);
            _dbContext.SaveChanges();
        }

        private bool DeleteCountry(Guid id)
        {
            var entityData = _dbContext.Country.IncludeRelated().FirstOrDefault(entity => entity.Id == id);
            if (entityData == null)
            {
                throw new ApplicationException("No data found!");
            }

            _dbContext.Country.Remove(entityData);
            _dbContext.SaveChanges();
            return true;
        }

        private void PatchCountry(Guid id, JsonPatchDocument<Country> updatedEntity)
        {
            if (updatedEntity == null)
            {
                throw new ApplicationException("Patch document is missing!");
            }

            var existingEntity = _dbContext.Country.FirstOrDefault(t => t.Id == id);
            if (existingEntity == null)
            {
                throw new ApplicationException("No data found!");
            }

            updatedEntity.ApplyTo(existingEntity);
            _dbContext.Country.Update(existingEntity);
            _dbContext.SaveChanges();
        }
        #endregion
    }
}