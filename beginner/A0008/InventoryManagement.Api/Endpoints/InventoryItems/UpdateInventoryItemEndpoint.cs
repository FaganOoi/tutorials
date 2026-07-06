using InventoryManagement.Api.Dtos.Requests;
using InventoryManagement.Api.Dtos.Responses;
using InventoryManagement.Application.InventoryItems.Orchestration;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Api.Endpoints.InventoryItems;


public static class UpdateInventoryItemEndpoint
{
    public static RouteGroupBuilder MapUpdateInventoryItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", async ([FromRoute] Guid id, [FromBody] UpdateInventoryItemRequest request, InventoryItemOrchestrationService service) =>
            {
                var item = await service.UpdateAsync(id, request.Name, request.Sku, request.Quantity);

                return item is null
                    ? Results.NotFound()
                    : Results.Ok(InventoryItemResponse.FromEntity(item));
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
