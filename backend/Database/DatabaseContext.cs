using System;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Database
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<SealIn> SealIn { get; set; }
        public virtual DbSet<SealItem> SealItem { get; set; }
        public virtual DbSet<SealType> SealType { get; set; }
        public virtual DbSet<SealStatus> SealStatus { get; set; }
        public virtual DbSet<Truck> Truck { get; set; }
        public virtual DbSet<SealOut> SealOut { get; set; }
        public virtual DbSet<SealOutInfo> SealOutInfo { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(e => e.Username).HasName("IDX_Users_Username").IsUnique();
                entity.HasAlternateKey(u => u.Username).HasName("UX_Users_Username");
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Updated).HasDefaultValueSql("(getdate())");
            });
            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasIndex(e => e.Name).HasName("AK_Role_Name").IsUnique();
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<SealIn>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Updated).HasDefaultValueSql("(getdate())");
            });
            modelBuilder.Entity<SealItem>(entity =>
            {
                entity.Property(e => e.IsUsed).HasDefaultValueSql("0");
                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Updated).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Truck>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("1");
                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Updated).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SealOut>(entity =>
            {
                entity.Property(e => e.IsCancel).HasDefaultValueSql("0");
                entity.Property(e => e.Created).HasDefaultValueSql("System");
                entity.Property(e => e.Updated).HasDefaultValueSql("System");
                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Updated).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<SealOutInfo>(entity =>
            {
                entity.Property(e => e.Created).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Updated).HasDefaultValueSql("(getdate())");
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
