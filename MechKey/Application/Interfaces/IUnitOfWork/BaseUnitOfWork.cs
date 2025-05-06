namespace Application.Interfaces.IUnitOfWork
{
    public interface BaseUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync(CancellationToken token = default);
        Task CommitAsync(CancellationToken token = default);
        Task RollbackAsync(CancellationToken token = default);
        Task<int> SaveChangesAsync();
    }
}
