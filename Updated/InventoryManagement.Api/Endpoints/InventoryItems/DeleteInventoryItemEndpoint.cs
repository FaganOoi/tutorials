using InventoryManagement.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Endpoints.InventoryItems;


public static class DeleteInventoryItemEndpoint
{
    public static RouteGroupBuilder MapDeleteInventoryItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", async ([FromRoute] Guid id, InventoryDbContext db) =>
            {
                var item = await db.InventoryItems.FindAsync(id);

                if (item is null)
                {
                    return Results.NotFound();
                }

                db.InventoryItems.Remove(item);
                await db.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("DeleteInventoryItem")
            .WithSummary("Delete an inventory item")
            .WithDescription("Deletes an inventory item from PostgreSQL.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}