using ECommerce.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data
{
    public class BaseDbContext : IdentityDbContext<ApplicationUser>
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) { }

        // Cấu hình thực hiện những câu lệnh thông qua DbSet tương tự ORM
        #region DbSet
        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<ProductOrder> ProductOrders { get; set; }

        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        #endregion

        // Fluent API in Entity Framework Core
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");
                entity.HasKey(order => order.Id);
                entity.Property(order => order.CreatedAt).HasDefaultValueSql("getutcdate()");
                entity.Property(order => order.Receiver).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<ProductOrder>(entity =>
            {
                entity.ToTable("ProductOrder");
                entity.HasKey(pOrder => new { pOrder.OrderId, pOrder.ProductId }); // Khóa chính có 2 cột
                entity.HasOne(pOrder => pOrder.Order)
                .WithMany(pOrder => pOrder.ProductOrders)
                .HasForeignKey(pOrder => pOrder.OrderId)
                .HasConstraintName("FK_ProductOrder_Order");

                entity.HasOne(pOrder => pOrder.Product)
                .WithMany(pOrder => pOrder.ProductOrders)
                .HasForeignKey(pOrder => pOrder.ProductId)
                .HasConstraintName("FK_ProductOrder_Product");
            });
        }
    }
}
