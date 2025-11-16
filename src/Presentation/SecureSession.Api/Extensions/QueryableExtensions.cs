using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace SecureSession.Api.Extensions
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public static class QueryableExtensions
    {
        public static Task<List<T>> ToMongoListAsync<T>(
            this IQueryable<T> source,
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            var query = predicate is null ? source : source.Where(predicate);

            return query.ToListAsync(cancellationToken);
        }

        public static Task<T> MongoFirstOrDefaultAsync<T>(
            this IQueryable<T> source,
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            var query = predicate is null ? source : source.Where(predicate);

            return query.FirstOrDefaultAsync(cancellationToken);
        }

        public static Task<bool> MongoAnyAsync<T>(
            this IQueryable<T> source,
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            if (predicate is null)
                return source.AnyAsync(cancellationToken);

            return source.Where(predicate).AnyAsync(cancellationToken);
        }
    }
}
