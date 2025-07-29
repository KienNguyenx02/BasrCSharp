using System.Linq;
using System.Linq.Dynamic.Core;
using WebApplication1.Shared.Results;

namespace WebApplication1.Shared.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFilterParams<T>(this IQueryable<T> query, FilterParams filterParams)
        {
            // Apply Search (assuming T has properties like Name or Description for searching)
            // This part needs to be generic or handled by specific services if search fields vary widely.
            // For now, let's assume a simple search on a 'Name' property if it exists.
            // A more robust solution would involve passing a search predicate or using reflection more carefully.
            // For demonstration, we'll keep it simple or rely on specific service implementations for complex search.

            // Apply Sort
            if (!string.IsNullOrWhiteSpace(filterParams.SortBy))
            {
                query = query.OrderBy(filterParams.SortBy);
            }
            else
            {
                // Default sort by Id if no sort is specified
                query = query.OrderBy("Id");
            }

            return query;
        }
    }
}