using LeSheet.Models;
using Microsoft.EntityFrameworkCore;

namespace LeSheet.Data;

public class AppDataContext : DbContext
{
  public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
  {
  }

  public DbSet<User> Users => Set<User>();
  public DbSet<Depense> Depenses => Set<Depense>();
  public DbSet<Remboursement> Remboursements => Set<Remboursement>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<User>(entity =>
    {
      entity.HasKey(u => u.Id);
      entity.Property(u => u.Name)
        .IsRequired()
        .HasMaxLength(100);
    });

    modelBuilder.Entity<Depense>(entity =>
    {
      entity.HasKey(d => d.Id);
      entity.Property(e => e.Description)
        .IsRequired()
        .HasMaxLength(512);
      entity.Property(e => e.Amount)
        .IsRequired()
        .HasPrecision(18, 2);
      entity.Property(e => e.Date)
        .HasDefaultValueSql("GETDATE()");

      entity.HasOne<User>()
        .WithMany()
        .HasForeignKey(e => e.PaidByUserId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<Remboursement>(entity =>
    {
      entity.Property(r => r.Amount)
        .IsRequired()
        .HasPrecision(18, 2);
      entity.HasOne<User>()
        .WithMany()
        .HasForeignKey(r => r.FromUserId)
        .OnDelete(DeleteBehavior.Restrict);
      entity.HasOne<User>()
        .WithMany()
        .HasForeignKey(r => r.ToUserId)
        .OnDelete(DeleteBehavior.Restrict);
      entity.Property(r => r.Date)
        .HasDefaultValueSql("GETDATE()");
    });
  }
}