using InventoryManagement.Domain.Abstractions;
using InventoryManagement.Domain.InventoryItems;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Data;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly InventoryDbContext db;

    public EfUnitOfWork(InventoryDbContext db)
    {
        this.db = db;
    }

    public async Task<IReadOnlyList<InventoryItemEntity>> ListInventoryItemsAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        IQueryable<InventoryItemEntity> query = db.InventoryItems
            .AsNoTrackingWithIdentityResolution();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                EF.Functions.ILike(item.Name, $"%{search}%") ||
                EF.Functions.ILike(item.Sku, $"%{search}%"));
        }

        return await query
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<InventoryItemEntity?> GetInventoryItemByIdReadOnlyAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return db.InventoryItems
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public Task<InventoryItemEntity?> GetInventoryItemByIdForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return db.InventoryItems
            .AsTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public async Task AddInventoryItemAsync(
        InventoryItemEntity item,
        CancellationToken cancellationToken = default)
    {
        await db.InventoryItems.AddAsync(item, cancellationToken);
    }

    public void RemoveInventoryItem(InventoryItemEntity item)
    {
        db.InventoryItems.Remove(item);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return db.SaveChangesAsync(cancellationToken);
    }
}
