var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();

var inventoryDb = postgres.AddDatabase("inventorydb");

builder.AddProject<Projects.InventoryManagement_Api>("api")
    .WithReference(inventoryDb)
    .WaitFor(inventoryDb);

builder.Build().Run();