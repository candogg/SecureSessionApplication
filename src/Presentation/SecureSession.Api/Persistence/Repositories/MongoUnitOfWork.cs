using MongoDB.Driver;
using SecureSession.Api.Contracts;

namespace SecureSession.Api.Persistence.Repositories
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public sealed class MongoUnitOfWork(IMongoClient client, IMongoDatabase database, IServiceProvider serviceProvider)
    : IMongoUnitOfWork
    {
        private readonly IMongoClient client = client;
        private readonly IMongoDatabase database = database;
        private readonly IServiceProvider serviceProvider = serviceProvider;
        private IClientSessionHandle? session;

        public IRepository<T> Repository<T>() where T : class
            => ActivatorUtilities.CreateInstance<MongoRepository<T>>(serviceProvider, database);

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (session != null) return;

            session = await client.StartSessionAsync(cancellationToken: cancellationToken);

            try
            {
                session.StartTransaction();
            }
            catch (NotSupportedException)
            {
                session.Dispose();
                session = null;
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (session == null) return;

            try
            {
                await session.CommitTransactionAsync(cancellationToken);
            }
            catch (NotSupportedException)
            { }
            finally
            {
                session.Dispose();
                session = null;
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (session == null) return;

            try
            {
                await session.AbortTransactionAsync(cancellationToken);
            }
            catch (NotSupportedException)
            { }
            finally
            {
                session.Dispose();
                session = null;
            }
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public async ValueTask DisposeAsync()
        {
            if (session != null)
            {
                await session.AbortTransactionAsync();
                session.Dispose();
                session = null;
            }
        }
    }
}
