using Microsoft.EntityFrameworkCore;
using StavZooApp.Models;

namespace StavZooApp.Data
{
    public class ZooDbContext : DbContext
    {
        public ZooDbContext(DbContextOptions<ZooDbContext> options) : base(options)
        {
        }

        public DbSet<Animal> Animals => Set<Animal>();
        public DbSet<AnimalMedia> AnimalMedias => Set<AnimalMedia>();
        public DbSet<AnimalDiaryEntry> DiaryEntries => Set<AnimalDiaryEntry>();
        public DbSet<Donation> Donations => Set<Donation>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Animal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Species).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MonthlyDonationGoal).HasPrecision(18, 2);
            });

            modelBuilder.Entity<AnimalMedia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Animal)
                      .WithMany(a => a.MediaItems)
                      .HasForeignKey(e => e.AnimalId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AnimalDiaryEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Animal)
                      .WithMany(a => a.DiaryEntries)
                      .HasForeignKey(e => e.AnimalId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<Donation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Animal)
                      .WithMany(a => a.Donations)
                      .HasForeignKey(e => e.AnimalId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            });
        }
    }
}
