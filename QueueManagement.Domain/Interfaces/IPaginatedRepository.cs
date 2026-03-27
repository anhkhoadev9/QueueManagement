using QueueManagement.Application.Common.GenericPage;
using QueueManagement.Domain.DTOs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Domain.Interfaces
{
    public interface IPaginatedRepository<T> where T : class
    {
        
        Task<PaginatedResult<TResult>> GetPaginatedAsync<TResult>(IQueryable<T> query, PaginationRequestDto requestDTO, Expression<Func<T, TResult>>? selector = null, CancellationToken cancellationToken = default);
    }
}
