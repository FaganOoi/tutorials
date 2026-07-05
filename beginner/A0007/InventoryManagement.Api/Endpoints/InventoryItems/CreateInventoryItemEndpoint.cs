using InventoryManagement.Api.Dtos.Requests;
using InventoryManagement.Api.Dtos.Responses;
using InventoryManagement.Application.InventoryItems;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Endpoints.InventoryItems;


public static class CreateInventoryItemEndpoint
{
    public static RouteGroupBuilder MapCreateInventoryItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", async ([FromBody] CreateInventoryItemRequest request, InventoryItemService service) =>
            {
                var item = await service.CreateAsync(request.Name, request.Sku, request.Quantity);

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
