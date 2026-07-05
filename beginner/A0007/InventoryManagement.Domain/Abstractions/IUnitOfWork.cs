using InventoryManagement.Domain.InventoryItems;

namespace InventoryManagement.Domain.Abstractions;

public interface IUnitOfWork
{
    Task<IReadOnlyList<InventoryItemEntity>> ListInventoryItemsAsync(
        string? search,
        CancellationToken cancellationToken = default);

    Task<InventoryItemEntity?> GetInventoryItemByIdReadOnlyAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<InventoryItemEntity?> GetInventoryItemByIdForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddInventoryItemAsync(
        InventoryItemEntity item,
        CancellationToken cancellationToken = default);

    void RemoveInventoryItem(InventoryItemEntity item);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
