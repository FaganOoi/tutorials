using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Api.Data.Entities;

public class InventoryItemEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}