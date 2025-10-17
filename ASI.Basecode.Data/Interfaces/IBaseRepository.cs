using System;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    /// <summary>
    /// Interface for base repository operations
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>Queryable of all entities</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Gets an entity by ID
        /// </summary>
        /// <param name="id">The entity ID</param>
        /// <returns>The entity if found, null otherwise</returns>
        TEntity GetById(string id);

        /// <summary>
        /// Adds a new entity
        /// </summary>
        /// <param name="entity">The entity to add</param>
        void Add(TEntity entity);

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">The entity to update</param>
        void Update(TEntity entity);

        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        void Delete(TEntity entity);
    }
}