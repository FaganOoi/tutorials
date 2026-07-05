using InventoryManagement.Domain.Abstractions;

namespace InventoryManagement.Domain.InventoryItems;

public class InventoryItemManager
{
    private readonly IUnitOfWork unitOfWork;

    public InventoryItemManager(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyList<InventoryItemEntity>> ListAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        return unitOfWork.ListInventoryItemsAsync(search, cancellationToken);
    }

    public Task<InventoryItemEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return unitOfWork.GetInventoryItemByIdReadOnlyAsync(id, cancellationToken);
    }

    public async Task<InventoryItemEntity> CreateAsync(
        string name,
        string sku,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var item = new InventoryItemEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            Sku = sku,
            Quantity = quantity
        };

        await unitOfWork.AddInventoryItemAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return item;
    }

    public async Task<InventoryItemEntity?> UpdateAsync(
        Guid id,
        string name,
        string sku,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.GetInventoryItemByIdForUpdateAsync(
            id,
            cancellationToken);

        if (item is null)
        {
            return null;
        }

        item.Name = name;
        item.Sku = sku;
        item.Quantity = quantity;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return item;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.GetInventoryItemByIdForUpdateAsync(
            id,
            cancellationToken);

        if (item is null)
        {
            return false;
        }

        unitOfWork.RemoveInventoryItem(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
