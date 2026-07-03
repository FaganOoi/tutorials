using InventoryManagement.Domain.InventoryItems;

namespace InventoryManagement.Application.InventoryItems;

public class InventoryItemService
{
    private readonly InventoryItemManager manager;

    public InventoryItemService(InventoryItemManager manager)
    {
        this.manager = manager;
    }

    public Task<IReadOnlyList<InventoryItemEntity>> ListAsync(string? search)
    {
        return manager.ListAsync(search);
    }

    public Task<InventoryItemEntity?> GetByIdAsync(Guid id)
    {
        return manager.GetByIdAsync(id);
    }

    public Task<InventoryItemEntity> CreateAsync(string name, string sku, int quantity)
    {
        return manager.CreateAsync(name, sku, quantity);
    }

    public Task<InventoryItemEntity?> UpdateAsync(Guid id, string name, string sku, int quantity)
    {
        return manager.UpdateAsync(id, name, sku, quantity);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return manager.DeleteAsync(id);
    }
}
