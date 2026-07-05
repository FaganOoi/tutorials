using System.ComponentModel.DataAnnotations;
using InventoryManagement.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<InventoryDbContext>("inventorydb");

//------//


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    await db.Database.MigrateAsync();
}

app.UseHttpsRedirection();

var inventoryGroup = app.MapGroup("/api/inventory-items")
    .WithTags("Inventory Items");

// List Items
inventoryGroup.MapGet("/", async ([FromQuery] string? search, InventoryDbContext db) =>
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

        return Results.Ok(items);
    })
    .WithName("ListInventoryItems")
    .WithSummary("List inventory items")
    .WithDescription("Returns inventory items from PostgreSQL. Use the optional search query to filter by name or SKU.")
    .Produces<List<InventoryItemEntity>>(StatusCodes.Status200OK)
    .AllowAnonymous();

// Get One Item
inventoryGroup.MapGet("/{id:guid}", async ([FromRoute] Guid id, InventoryDbContext db) =>
    {
        var item = await db.InventoryItems.FindAsync(id);

        return item is null ? Results.NotFound() : Results.Ok(item);
    })
    .WithName("GetInventoryItemById")
    .WithSummary("Get one inventory item")
    .WithDescription("Returns one inventory item from PostgreSQL by id if it exists.")
    .Produces<InventoryItemEntity>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .AllowAnonymous();

// Create item
inventoryGroup.MapPost("/", async ([FromBody] CreateInventoryItemRequest request, InventoryDbContext db) =>
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

        return Results.Created($"/api/inventory-items/{item.Id}", item);
    })
    .WithName("CreateInventoryItem")
    .WithSummary("Create an inventory item")
    .WithDescription("Creates a new inventory item and saves it to PostgreSQL.")
    .Accepts<CreateInventoryItemRequest>("application/json")
    .Produces<InventoryItemEntity>(StatusCodes.Status201Created);

// Update Item
inventoryGroup.MapPut("/{id:guid}", async ([FromRoute] Guid id, [FromBody] UpdateInventoryItemRequest request, InventoryDbContext db) =>
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

        return Results.Ok(item);
    })
    .WithName("UpdateInventoryItem")
    .WithSummary("Update an inventory item")
    .WithDescription("Updates an existing inventory item in PostgreSQL.")
    .Accepts<UpdateInventoryItemRequest>("application/json")
    .Produces<InventoryItemEntity>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

// Delete Item
inventoryGroup.MapDelete("/{id:guid}", async ([FromRoute] Guid id, InventoryDbContext db) =>
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


app.Run();

class InventoryItemEntity
{
    [Key]
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