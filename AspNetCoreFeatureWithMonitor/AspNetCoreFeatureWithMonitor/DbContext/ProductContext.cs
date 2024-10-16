using Microsoft.EntityFrameworkCore;

namespace AspNetCoreFeatureWithMonitor.DbContext;

public class ProductContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Order> Orders { get; set; }
    
    
    public DbSet<OrderDetail> OrderDetails { get; set; }
    
    public DbSet<Product> Products { get; set; }
}