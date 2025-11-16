using MongoDB.Bson;
using MongoDB.Driver;
using SecureSession.Api.Contracts;
using System.Linq.Expressions;

namespace SecureSession.Api.Persistence.Repositories
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class MongoRepository<T>(IMongoDatabase db)
    : IRepository<T> where T : class
    {
        private readonly IMongoCollection<T> col = db.GetCollection<T>(typeof(T).Name);

        public IQueryable<T> Query() => col.AsQueryable();

        public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken)
        {
            FilterDefinition<T> filter;

            if (ObjectId.TryParse(id.ToString(), out var oid))
            {
                filter = Builders<T>.Filter.Eq("_id", oid);
            }
            else
            {
                filter = Builders<T>.Filter.Eq("_id", id);
            }

            return await col.Find(filter).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public Task InsertAsync(T entity, CancellationToken cancellationToken) => col.InsertOneAsync(entity, cancellationToken: cancellationToken);

        public Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            var idProp = typeof(T).GetProperty("Id") ?? throw new InvalidOperationException("Entity must have an Id property for update.");

            var idValue = idProp.GetValue(entity) ?? throw new InvalidOperationException("Entity Id is null.");

            FilterDefinition<T> filter;

            if (ObjectId.TryParse(idValue.ToString(), out var oid))
            {
                filter = Builders<T>.Filter.Eq("_id", oid);
            }
            else
            {
                filter = Builders<T>.Filter.Eq("_id", idValue);
            }

            return col.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
        }

        public Task DeleteAsync(object id, CancellationToken cancellationToken)
        {
            FilterDefinition<T> filter;

            if (ObjectId.TryParse(id.ToString(), out var oid))
            {
                filter = Builders<T>.Filter.Eq("_id", oid);
            }
            else
            {
                filter = Builders<T>.Filter.Eq("_id", id);
            }

            return col.DeleteOneAsync(filter, cancellationToken);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await col
                .Find(predicate)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await col.Find(predicate).Limit(1).AnyAsync(cancellationToken);

        public Task<List<T>> ToListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => (predicate is null ? col.Find(_ => true) : col.Find(predicate)).ToListAsync(cancellationToken);
    }
}
