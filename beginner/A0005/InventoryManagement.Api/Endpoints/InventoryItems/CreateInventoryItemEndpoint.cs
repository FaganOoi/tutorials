using InventoryManagement.Api.Data;
using InventoryManagement.Api.Data.Entities;
using InventoryManagement.Api.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Endpoints.InventoryItems;


public static class CreateInventoryItemEndpoint
{
    public static RouteGroupBuilder MapCreateInventoryItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async ([FromBody] CreateInventoryItemRequest request, InventoryDbContext db) =>
            {
                var item = new InventoryItemEntity
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Sku = request.Sku,
                    Quantity = request.Quantity
                };

                db.InventoryItems.Add(item);
                await db.SaveChangesAsync();

                var response = InventoryItemResponse.FromEntity(item);

                return Results.Created($"/api/inventory-items/{item.Id}", response);
            })
            .WithName("CreateInventoryItem")
            .WithSummary("Create an inventory item")
            .WithDescription("Creates a new inventory item and saves it to PostgreSQL.")
            .Accepts<CreateInventoryItemRequest>("application/json")
            .Produces<InventoryItemResponse>(StatusCodes.Status201Created);

        return group;
    }
}