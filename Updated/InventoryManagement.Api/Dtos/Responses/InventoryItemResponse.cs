using InventoryManagement.Api.Data.Entities;

namespace InventoryManagement.Api.Dtos.Responses;

public class InventoryItemResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }

    public static InventoryItemResponse FromEntity(InventoryItemEntity entity)
    {
        return new InventoryItemResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Sku = entity.Sku,
            Quantity = entity.Quantity
        };
    }
}