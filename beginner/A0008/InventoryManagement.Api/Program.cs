using InventoryManagement.Api.Endpoints.InventoryItems;
using InventoryManagement.Application.InventoryItems.Orchestration;
using InventoryManagement.Application.InventoryItems.Processing;
using InventoryManagement.Domain.InventoryItems;
using InventoryManagement.Infrastructure;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("inventorydb")
    ?? throw new InvalidOperationException("Connection string 'inventorydb' was not found.");

builder.Services.AddInfrastructure(connectionString);
builder.EnrichNpgsqlDbContext<InventoryDbContext>();

builder.Services.AddScoped<InventoryItemManager>();
builder.Services.AddScoped<InventoryItemProcessingService>();
builder.Services.AddScoped<InventoryItemOrchestrationService>();

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
