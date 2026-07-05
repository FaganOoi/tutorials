using InventoryManagement.Api.Dtos.Responses;
using InventoryManagement.Application.InventoryItems;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Endpoints.InventoryItems;

public static class ListInventoryItemsEndpoint
{
    public static RouteGroupBuilder MapListInventoryItemsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async ([FromQuery] string? search, InventoryItemService service) =>
            {
                var items = await service.ListAsync(search);

                var response = items
                    .Select(InventoryItemResponse.FromEntity)
                    .ToList();

                return Results.Ok(response);
            })
            .WithName("ListInventoryItems")
            .WithSummary("List inventory items")
            .WithDescription("Returns inventory items from PostgreSQL. Use the optional search query to filter by name or SKU.")
            .Produces<List<InventoryItemResponse>>(StatusCodes.Status200OK)
            .AllowAnonymous();

        return group;
    }
}
