using Microsoft.EntityFrameworkCore;

namespace YDeveloper.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<Models.Pagination.PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query, 
            int pageNumber, 
            int pageSize)
        {
            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Models.Pagination.PagedResult<T>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
    }
}
