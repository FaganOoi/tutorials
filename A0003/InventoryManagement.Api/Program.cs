using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var inventoryItems = new List<InventoryItem>
{
    new()
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Name = "Mechanical Keyboard",
        Sku = "KB-001",
        Quantity = 10
    },
    new()
    {
        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        Name = "USB-C Cable",
        Sku = "CBL-001",
        Quantity = 25
    }
};

var inventoryGroup = app.MapGroup("/api/inventory-items")
    .WithTags("Inventory Items");

inventoryGroup.MapGet("/", ([FromQuery] string? search) =>
{
    var results = inventoryItems.AsEnumerable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        results = results.Where(x =>
            x.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            x.Sku.Contains(search, StringComparison.OrdinalIgnoreCase));
    }

    return Results.Ok(results.ToList());
})
    .WithName("ListInventoryItems")
    .WithSummary("List inventory items")
    .WithDescription("Returns inventory items currently stored in memory. Use the optional search query to filter by name or SKU.")
    .Produces<List<InventoryItem>>(StatusCodes.Status200OK)
    .AllowAnonymous();

inventoryGroup.MapGet("/{id:guid}", ([FromRoute] Guid id) =>
{
    var item = inventoryItems.FirstOrDefault(x => x.Id == id);

    return item is null ? Results.NotFound() : Results.Ok(item);
})
    .WithName("GetInventoryItemById")
    .WithSummary("Get one inventory item")
    .WithDescription("Returns one inventory item by id if it exists.")
    .Produces<InventoryItem>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .AllowAnonymous();

// Later, this endpoint should require a logged-in user.
inventoryGroup.MapPost("/", ([FromBody] CreateInventoryItemRequest request) =>
{
    var item = new InventoryItem
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        Sku = request.Sku,
        Quantity = request.Quantity
    };

    inventoryItems.Add(item);

    return Results.Created($"/api/inventory-items/{item.Id}", item);
})
    .WithName("CreateInventoryItem")
    .WithSummary("Create an inventory item")
    .WithDescription("Creates a new inventory item. In a real application, this endpoint should require authentication.")
    .Accepts<CreateInventoryItemRequest>("application/json")
    .Produces<InventoryItem>(StatusCodes.Status201Created);

// Later, this endpoint should require a logged-in user.
inventoryGroup.MapPut("/{id:guid}", ([FromRoute] Guid id, [FromBody] UpdateInventoryItemRequest request) =>
{
    var index = inventoryItems.FindIndex(x => x.Id == id);

    if (index == -1)
    {
        return Results.NotFound();
    }

    var updatedItem = new InventoryItem
    {
        Id = id,
        Name = request.Name,
        Sku = request.Sku,
        Quantity = request.Quantity
    };

    inventoryItems[index] = updatedItem;

    return Results.Ok(updatedItem);
})
    .WithName("UpdateInventoryItem")
    .WithSummary("Update an inventory item")
    .WithDescription("Updates an existing inventory item. In a real application, this endpoint should require authentication.")
    .Accepts<UpdateInventoryItemRequest>("application/json")
    .Produces<InventoryItem>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

// Later, this endpoint should require a logged-in user.
inventoryGroup.MapDelete("/{id:guid}", ([FromRoute] Guid id) =>
{
    var item = inventoryItems.FirstOrDefault(x => x.Id == id);

    if (item is null)
    {
        return Results.NotFound();
    }

    inventoryItems.Remove(item);

    return Results.NoContent();
})
    .WithName("DeleteInventoryItem")
    .WithSummary("Delete an inventory item")
    .WithDescription("Deletes an inventory item. In a real application, this endpoint should require authentication.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);

app.Run();

class InventoryItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

class CreateInventoryItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

class UpdateInventoryItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public int Quantity { get; set; }
}