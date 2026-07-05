using InventoryManagement.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Domain.InventoryItems;

public class InventoryItemManager
{
    private readonly InventoryDbContext db;

    public InventoryItemManager(InventoryDbContext db)
    {
        this.db = db;
    }

    public async Task<IReadOnlyList<InventoryItemEntity>> ListAsync(string? search)
    {
        var query = db.InventoryItems.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";

            query = query.Where(x =>
                EF.Functions.ILike(x.Name, pattern) ||
                EF.Functions.ILike(x.Sku, pattern));
        }

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public Task<InventoryItemEntity?> GetByIdAsync(Guid id)
    {
        return db.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<InventoryItemEntity> CreateAsync(string name, string sku, int quantity)
    {
        var item = new InventoryItemEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            Sku = sku,
            Quantity = quantity
        };

        db.InventoryItems.Add(item);
        await db.SaveChangesAsync();

        return item;
    }

    public async Task<InventoryItemEntity?> UpdateAsync(Guid id, string name, string sku, int quantity)
    {
        var item = await GetByIdAsync(id);

        if (item is null)
        {
            return null;
        }

        item.Name = name;
        item.Sku = sku;
        item.Quantity = quantity;

        await db.SaveChangesAsync();

        return item;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await GetByIdAsync(id);

        if (item is null)
        {
            return false;
        }

        db.InventoryItems.Remove(item);
        await db.SaveChangesAsync();

        return true;
    }
}
