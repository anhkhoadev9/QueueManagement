
using QueueManagement.Domain.Entities;
using QueueManagement.Domain.Entities.Abstractions;
//using QueueManagement.Application.Common.GenericPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> Query();  
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<T?> GetByQueueTicketIdAsync(Guid id, CancellationToken cancellationToken = default);
       
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);
        void Delete(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
