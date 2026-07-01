namespace InventoryManagement.Api.Endpoints.InventoryItems;

public static class InventoryItemEndpoints
{
    public static IEndpointRouteBuilder MapInventoryItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inventory-items")
            .WithTags("Inventory Items");

        group.MapListInventoryItemsEndpoint();
        group.MapGetInventoryItemByIdEndpoint();
        group.MapCreateInventoryItemEndpoint();
        group.MapUpdateInventoryItemEndpoint();
        group.MapDeleteInventoryItemEndpoint();

        return app;
    }
}