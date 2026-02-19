using LeSheet.Models;
using Microsoft.EntityFrameworkCore;

namespace LeSheet.Data;

public class AppDataContext : DbContext
{
  public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) {}

  public DbSet<User> Users => Set<User>();
  public DbSet<Depense> Depenses => Set<Depense>();
  public DbSet<Remboursement> Remboursements => Set<Remboursement>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Depense>()
      .Property(d => d.Amount)
      .HasPrecision(18, 2);
    
    modelBuilder.Entity<Remboursement>()
      .Property(r => r.Amount)
      .HasPrecision(18, 2);
  }
}