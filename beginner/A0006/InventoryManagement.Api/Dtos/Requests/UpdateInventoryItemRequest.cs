namespace InventoryManagement.Api.Dtos.Requests;

public class UpdateInventoryItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}