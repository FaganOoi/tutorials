using InventoryManagement.Api.Data;
using InventoryManagement.Api.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Endpoints.InventoryItems;


public static class GetInventoryItemByIdEndpoint
{
    public static RouteGroupBuilder MapGetInventoryItemByIdEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async ([FromRoute] Guid id, InventoryDbContext db) =>
            {
                var item = await db.InventoryItems.FindAsync(id);

                return item is null
                    ? Results.NotFound()
                    : Results.Ok(InventoryItemResponse.FromEntity(item));
            })
            .WithName("GetInventoryItemById")
            .WithSummary("Get one inventory item")
            .WithDescription("Returns one inventory item from PostgreSQL by id if it exists.")
            .Produces<InventoryItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .AllowAnonymous();

        return group;
    }
}