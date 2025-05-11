namespace Domain.Common
{
    public interface BaseRepository<T> where T : class
    {
        Task<T> CreateAsync(T entity, CancellationToken token = default);
        Task<T> UpdateAsync(T entity, CancellationToken token = default);
        Task DeleteAsync(T entity, CancellationToken token = default);
        Task<T> GetByIdAsync(Guid id, CancellationToken token = default);
        IQueryable<T> GetAllAsync();
    }
}
