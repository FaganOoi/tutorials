using InventoryManagement.Api.Endpoints.InventoryItems;
using InventoryManagement.Application.InventoryItems;
using InventoryManagement.Domain.Data;
using InventoryManagement.Domain.InventoryItems;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("inventorydb")
    ?? throw new InvalidOperationException("Connection string 'inventorydb' was not found.");

builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("InventoryManagement.Api.Migrations");
    });
});

builder.EnrichNpgsqlDbContext<InventoryDbContext>();

builder.Services.AddScoped<InventoryItemManager>();
builder.Services.AddScoped<InventoryItemService>();

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

app.MapInventoryItemEndpoints();

app.Run();
