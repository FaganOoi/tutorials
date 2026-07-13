using InventoryManagement.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryManagement.Infrastructure.Data;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly InventoryDbContext db;
    private IDbContextTransaction? currentTransaction;

    public EfUnitOfWork(InventoryDbContext db)
    {
        this.db = db;
    }

    public async Task BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentTransaction is not null)
        {
            throw new InvalidOperationException("A transaction is already active.");
        }

        currentTransaction = await db.Database.BeginTransactionAsync(
            cancellationToken);
    }

    public async Task CommitTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentTransaction is null)
        {
            return;
        }

        await currentTransaction.CommitAsync(cancellationToken);
        await currentTransaction.DisposeAsync();
        currentTransaction = null;
    }

    public async Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        if (currentTransaction is null)
        {
            return;
        }

        await currentTransaction.RollbackAsync(cancellationToken);
        await currentTransaction.DisposeAsync();
        currentTransaction = null;
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync<TEntity>(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> query,
        bool isReadOnly = true,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        IQueryable<TEntity> dbQuery = CreateQuery<TEntity>(isReadOnly);

        return await query(dbQuery).ToListAsync(cancellationToken);
    }

    public Task<TEntity?> FirstOrDefaultAsync<TEntity>(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> query,
        bool isReadOnly = true,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        IQueryable<TEntity> dbQuery = CreateQuery<TEntity>(isReadOnly);

        return query(dbQuery).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        await db.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public void Remove<TEntity>(TEntity entity)
        where TEntity : class
    {
        db.Set<TEntity>().Remove(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return db.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<TEntity> CreateQuery<TEntity>(bool isReadOnly)
        where TEntity : class
    {
        IQueryable<TEntity> query = db.Set<TEntity>();

        if (isReadOnly)
        {
            query = query.AsNoTrackingWithIdentityResolution();
        }

        return query;
    }
}
