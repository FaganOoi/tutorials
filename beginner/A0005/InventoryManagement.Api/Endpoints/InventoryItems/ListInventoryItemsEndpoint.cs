using InventoryManagement.Api.Data;
using InventoryManagement.Api.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Endpoints.InventoryItems;

public static class ListInventoryItemsEndpoint
{
    public static RouteGroupBuilder MapListInventoryItemsEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async ([FromQuery] string? search, InventoryDbContext db) =>
            {
                var query = db.InventoryItems.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var pattern = $"%{search}%";

                    query = query.Where(x =>
                        EF.Functions.ILike(x.Name, pattern) ||
                        EF.Functions.ILike(x.Sku, pattern));
                }

                var items = await query
                    .OrderBy(x => x.Name)
                    .ToListAsync();

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