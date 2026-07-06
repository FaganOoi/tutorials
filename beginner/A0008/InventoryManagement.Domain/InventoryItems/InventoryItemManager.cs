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
        return unitOfWork.ListAsync<InventoryItemEntity>(
            query =>
            {
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(item =>
                        item.Name.Contains(search) ||
                        item.Sku.Contains(search));
                }

                return query.OrderBy(item => item.Name);
            },
            cancellationToken: cancellationToken);
    }

    public Task<InventoryItemEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return unitOfWork.FirstOrDefaultAsync<InventoryItemEntity>(
            query => query.Where(item => item.Id == id),
            cancellationToken: cancellationToken);
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

        await unitOfWork.AddAsync(item, cancellationToken);

        return item;
    }

    public async Task<InventoryItemEntity?> UpdateAsync(
        Guid id,
        string name,
        string sku,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.FirstOrDefaultAsync<InventoryItemEntity>(
            query => query.Where(item => item.Id == id),
            isReadOnly: false,
            cancellationToken: cancellationToken);

        if (item is null)
        {
            return null;
        }

        item.Name = name;
        item.Sku = sku;
        item.Quantity = quantity;

        return item;
    }

    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var item = await unitOfWork.FirstOrDefaultAsync<InventoryItemEntity>(
            query => query.Where(item => item.Id == id),
            isReadOnly: false,
            cancellationToken: cancellationToken);

        if (item is null)
        {
            return false;
        }

        unitOfWork.Remove(item);

        return true;
    }
}
