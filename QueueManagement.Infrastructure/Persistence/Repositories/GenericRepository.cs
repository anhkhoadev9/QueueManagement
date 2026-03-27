using Microsoft.EntityFrameworkCore;
using QueueManagement.Application.Exceptions;
using QueueManagement.Domain.Entities.Abstractions;
using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Domain.Interfaces;
using QueueManagement.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using QueueManagement.Application.Common.Interfaces;
using QueueManagement.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QueueManagement.Application.DTOs;
using QueueManagement.Domain.DTOs;

namespace QueueManagement.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>, IPaginatedRepository<T> where T : BaseEntity
    {
        protected readonly QueueManagementDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(QueueManagementDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)

        {

            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(predicate, cancellationToken);
        }

        public async Task<T?> GetByQueueTicketIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public IQueryable<T> Query()
        {
            return _dbSet.AsNoTracking();//không có Trace Tracking
        }

        public async Task<PaginatedResult<TResult>> GetPaginatedAsync<TResult>(IQueryable<T> query, PaginationRequestDto requestDTO, Expression<Func<T, TResult>>? selector = null, CancellationToken cancellationToken = default)
        {



            requestDTO.PageNumber = requestDTO.PageNumber <= 0 ? 1 : requestDTO.PageNumber;
            requestDTO.PageSize = requestDTO.PageSize <= 0 ? 10 : requestDTO.PageSize > requestDTO.MaxPageSize ? requestDTO.MaxPageSize : requestDTO.PageSize;


            if (selector == null)
            {
                throw new InvalidOperationException("Selector is required for projection");
            }

            var items = await query.ToPagedResultAsync(requestDTO.PageNumber, requestDTO.PageSize, requestDTO.MaxPageSize, selector, cancellationToken);

            return items;
        }


    }
}
