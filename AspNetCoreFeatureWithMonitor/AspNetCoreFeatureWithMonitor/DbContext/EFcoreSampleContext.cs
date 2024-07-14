using Microsoft.EntityFrameworkCore;

namespace AspNetCoreFeatureWithMonitor.DbContext;

public class EFcoreSampleContext : Microsoft.EntityFrameworkCore.DbContext
{
    public EFcoreSampleContext(DbContextOptions<EFcoreSampleContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<OrderDetail> OrderDetails { get; set; }
    
    public DbSet<Product> Products { get; set; }
}