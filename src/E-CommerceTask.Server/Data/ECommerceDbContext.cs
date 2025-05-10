namespace E_CommerceTask.Server.Data;

public class ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products { get; init; }
    public DbSet<ProductCategory> Categories { get; init; }
    public DbSet<Purchase> Purchases { get; init; }
    public DbSet<User> Users { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>();
        modelBuilder.Entity<ProductCategory>();
        modelBuilder.Entity<Purchase>();
        modelBuilder.Entity<User>();
    }
}