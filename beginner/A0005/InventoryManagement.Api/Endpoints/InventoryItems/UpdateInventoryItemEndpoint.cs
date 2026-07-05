using InventoryManagement.Api.Data;
using InventoryManagement.Api.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Endpoints.InventoryItems;


public static class UpdateInventoryItemEndpoint
{
    public static RouteGroupBuilder MapUpdateInventoryItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async ([FromRoute] Guid id, [FromBody] UpdateInventoryItemRequest request, InventoryDbContext db) =>
            {
                var item = await db.InventoryItems.FindAsync(id);

                if (item is null)
                {
                    return Results.NotFound();
                }

                item.Name = request.Name;
                item.Sku = request.Sku;
                item.Quantity = request.Quantity;

                await db.SaveChangesAsync();

                return Results.Ok(InventoryItemResponse.FromEntity(item));
            })
            .WithName("UpdateInventoryItem")
            .WithSummary("Update an inventory item")
            .WithDescription("Updates an existing inventory item in PostgreSQL.")
            .Accepts<UpdateInventoryItemRequest>("application/json")
            .Produces<InventoryItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}