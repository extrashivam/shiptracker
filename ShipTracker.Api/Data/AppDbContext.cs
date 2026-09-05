using Microsoft.EntityFrameworkCore;
using ShipTracker.Api.Models;

namespace ShipTracker.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Ship> Ships => Set<Ship>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Ship>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.Mmsi).HasMaxLength(16).IsRequired();
            e.Property(s => s.Name).HasMaxLength(128);
            e.Property(s => s.Destination).HasMaxLength(128);
            e.HasIndex(s => s.Mmsi).IsUnique();
            e.HasIndex(s => s.LastUpdatedUtc);
        });
    }
}