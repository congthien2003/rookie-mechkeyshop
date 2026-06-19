using Domain.Common;
using MechkeyShop.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public abstract class RepositoryBase<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    protected RepositoryBase(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    protected static void SetAuditFields(T entity)
    {
        if (entity is BaseEntity baseEntity)
        {
            if (baseEntity.Id == Guid.Empty)
                baseEntity.Id = Guid.NewGuid();
            if (baseEntity.CreatedAt == default)
                baseEntity.CreatedAt = DateTime.UtcNow;
            baseEntity.LastUpdatedAt = DateTime.UtcNow;
        }
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
        {
            return await _dbSet.OfType<BaseEntity>()
                .Where(e => e.Id == id && !e.IsDeleted)
                .Cast<T>()
                .FirstOrDefaultAsync(ct);
        }
        return await _dbSet.FindAsync(new object[] { id }, ct);
    }

    public virtual IQueryable<T> GetAllAsync()
        => _dbSet.AsQueryable();

    // --- Sync: chỉ thao tác DbSet, KHÔNG SaveChanges ---
    public virtual void Add(T entity)
    {
        SetAuditFields(entity);
        _dbSet.Add(entity);
    }

    public virtual void Update(T entity)
    {
        SetAuditFields(entity);
        _dbSet.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
        {
            var baseEntity = entity as BaseEntity;
            baseEntity!.IsDeleted = true;
            baseEntity.LastUpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }
        else
        {
            _dbSet.Remove(entity);
        }
    }

    // --- Async: tự động SaveChanges ---
    public virtual async Task<T> CreateAsync(T entity, CancellationToken ct = default)
    {
        Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity, CancellationToken ct = default)
    {
        Update(entity);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        Delete(entity);
        await _context.SaveChangesAsync(ct);
    }
}
