namespace SecureSession.Api.Contracts
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public interface IMongoUnitOfWork : IAsyncDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
