using Microsoft.EntityFrameworkCore;

namespace ShopApi.Data;

public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.CustomerEmail).IsRequired().HasMaxLength(200);
            e.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<OrderLine>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Quantity).IsRequired();
            e.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        });
    }
}

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}

public class Order
{
    public Guid Id { get; set; }
    public string CustomerEmail { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
    public List<OrderLine> Lines { get; set; } = new();
}

public class OrderLine
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public int Quantity { get; set; }
}
