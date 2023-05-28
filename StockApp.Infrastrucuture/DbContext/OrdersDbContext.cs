using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace StockApp.Infrastructure.Repositories;

public class OrdersDbContext : DbContext
{
    public DbSet<BuyOrder> BuyOrders { get; set; }
    public DbSet<SellOrder> SellOrders { get; set; }

    public OrdersDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BuyOrder>().ToTable("BuyOrders");
        modelBuilder.Entity<SellOrder>().ToTable("SellOrders");
    }
}