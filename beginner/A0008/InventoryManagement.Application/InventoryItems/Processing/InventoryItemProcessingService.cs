using InventoryManagement.Domain.InventoryItems;

namespace InventoryManagement.Application.InventoryItems.Processing;

public class InventoryItemProcessingService
{
    private readonly InventoryItemManager manager;

    public InventoryItemProcessingService(InventoryItemManager manager)
    {
        this.manager = manager;
    }

    public Task<IReadOnlyList<InventoryItemEntity>> ListAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        return manager.ListAsync(search, cancellationToken);
    }

    public Task<InventoryItemEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return manager.GetByIdAsync(id, cancellationToken);
    }

    public Task<InventoryItemEntity> CreateAsync(
        string name,
        string sku,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        return manager.CreateAsync(name, sku, quantity, cancellationToken);
    }

    public Task<InventoryItemEntity?> UpdateAsync(
        Guid id,
        string name,
        string sku,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        return manager.UpdateAsync(id, name, sku, quantity, cancellationToken);
    }

    public Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return manager.DeleteAsync(id, cancellationToken);
    }
}
