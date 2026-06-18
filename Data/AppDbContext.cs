using EventBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EventBookingAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    
    public DbSet<User> Users { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Event>()
            .Property(e => e.Price)
            .HasPrecision(10, 2);
        
        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalPrice)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<User>()
            .HasMany(n => n.Bookings)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId);
        
        modelBuilder.Entity<Event>()
            .HasMany(n => n.Bookings)
            .WithOne(n => n.Event)
            .HasForeignKey(n => n.EventId);
    }
}