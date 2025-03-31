using Com.Store.Orders.Domain.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Data;
using Com.Store.Orders.Domain.Data.Enums;

namespace Com.Store.Orders.Domain.Data
{
    public class OrdersDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }

        public DbSet<User> Users { get; set; }

        public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("orders");
            ChangeCase(modelBuilder);

            modelBuilder.Entity<Order>()
                .Property(x => x.Status)
                .HasConversion<string>();
            modelBuilder.Entity<Order>()
                .HasMany(x => x.Items)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId);
            modelBuilder.Entity<Order>()
                .Property(x => x.CreatedAt)
                .HasColumnType("timestamp");
            modelBuilder.Entity<Order>()
                .Property(x => x.UpdatedAt)
                .HasColumnType("timestamp");
            modelBuilder.Entity<Order>()
                .Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<Item>()
                .Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<OrderItem>()
                .HasOne(x => x.Item)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.ItemId);
            modelBuilder.Entity<OrderItem>()
                .Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<User>()
                .Property(x => x.IsDeleted)
                .HasDefaultValue(false);
            modelBuilder.Entity<User>()
                .Property(x => x.CreatedAt)
                .HasColumnType("timestamp");
            modelBuilder.Entity<User>()
                .Property(x => x.UpdatedAt)
                .HasColumnType("timestamp");
            modelBuilder.Entity<User>()
                .Property(x => x.Roles)
                .HasColumnType("text[]");
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("idx_users_email");
            modelBuilder.Entity<User>()
                .HasIndex(x => new { x.Email, x.PasswordHash })
                .HasDatabaseName("idx_users_email_password_hash");
        }

        private void ChangeCase(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(ToSnakeCase(entity.GetTableName()));

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.GetColumnName()));
                }
            }
        }

        private static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }
}
