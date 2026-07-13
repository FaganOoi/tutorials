namespace InventoryManagement.Domain.Abstractions;

public interface IUnitOfWork
{
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> ListAsync<TEntity>(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> query,
        bool isReadOnly = true,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task<TEntity?> FirstOrDefaultAsync<TEntity>(
        Func<IQueryable<TEntity>, IQueryable<TEntity>> query,
        bool isReadOnly = true,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task AddAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    void Remove<TEntity>(TEntity entity)
        where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
