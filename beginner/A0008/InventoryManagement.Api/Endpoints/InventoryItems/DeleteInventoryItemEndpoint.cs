using InventoryManagement.Application.InventoryItems.Orchestration;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Endpoints.InventoryItems;


public static class DeleteInventoryItemEndpoint
{
    public static RouteGroupBuilder MapDeleteInventoryItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", async ([FromRoute] Guid id, InventoryItemOrchestrationService service) =>
            {
                var deleted = await service.DeleteAsync(id);

                return deleted
                    ? Results.NoContent()
                    : Results.NotFound();
            })
            .WithName("DeleteInventoryItem")
            .WithSummary("Delete an inventory item")
            .WithDescription("Deletes an inventory item from PostgreSQL.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
