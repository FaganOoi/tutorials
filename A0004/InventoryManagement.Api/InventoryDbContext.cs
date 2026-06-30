using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api;

class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<InventoryItemEntity> InventoryItems => Set<InventoryItemEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryItemEntity>(entity =>
        {
            entity.ToTable("inventory_items");
        });
    }
}