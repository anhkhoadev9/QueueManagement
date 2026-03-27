using Microsoft.EntityFrameworkCore;
using QueueManagement.Application.Common.GenericPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagement.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<TResult>> ToPagedResultAsync<T, TResult>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize,
            int maxPageSize,
            Expression<Func<T, TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize > maxPageSize ? maxPageSize : pageSize;

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(selector)
                .ToListAsync(cancellationToken);

            return new PaginatedResult<TResult>(items, pageNumber, pageSize, totalCount);
        }
    }
}
