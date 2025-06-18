using Microsoft.EntityFrameworkCore;
using ERP.Domain.Entities;

namespace ERP.Infrastructure.Data
{
    public class ERPDbContext : DbContext
    {
        public ERPDbContext(DbContextOptions<ERPDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Customer 설정
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
            });

            // Product 설정
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.SKU).IsUnique();
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
            });

            // Inventory 설정 - 일대일 관계로 수정
            modelBuilder.Entity<Inventory>(entity =>
            {
                // 기본키 설정
                entity.HasKey(e => e.Id);

                // Product 관계 설정 - 일대일 관계로 변경
                entity.HasOne(e => e.Product)
                      .WithOne(p => p.Inventory)
                      .HasForeignKey<Inventory>(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Warehouse 관계 설정 - 명시적으로 외래키 지정
                entity.HasOne(e => e.Warehouse)
                      .WithMany(w => w.Inventories)
                      .HasForeignKey(e => e.WarehouseId)
                      .OnDelete(DeleteBehavior.Cascade);

                // 복합 인덱스 설정
                entity.HasIndex(e => new { e.ProductId, e.WarehouseId })
                      .IsUnique()
                      .HasDatabaseName("IX_Inventory_ProductId_WarehouseId");

                // 프로퍼티 설정
                entity.Property(e => e.Quantity).HasDefaultValue(0);
                entity.Property(e => e.MinimumStock).HasDefaultValue(0);
                entity.Property(e => e.LastUpdated).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // 외래키 컬럼명 명시적 지정
                entity.Property(e => e.ProductId).HasColumnName("ProductId");
                entity.Property(e => e.WarehouseId).HasColumnName("WarehouseId");
            });

            // Warehouse 설정
            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Location).HasMaxLength(500);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // InventoryTransaction 설정
            modelBuilder.Entity<InventoryTransaction>(entity =>
            {
                entity.Property(e => e.TransactionType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId);
                entity.HasOne(e => e.Warehouse)
                      .WithMany()
                      .HasForeignKey(e => e.WarehouseId);
            });

            // Order 설정
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
                // MySQL에서는 UTC() 함수 사용
                entity.Property(e => e.OrderDate).HasDefaultValueSql("UTC_TIMESTAMP()");
                entity.HasOne(e => e.Customer)
                      .WithMany()
                      .HasForeignKey(e => e.CustomerId);
            });

            // OrderDetail 설정
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Quantity).IsRequired();
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderDetails)
                      .HasForeignKey(e => e.OrderId);
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId);
                entity.HasOne(e => e.Warehouse)
                      .WithMany()
                      .HasForeignKey(e => e.WarehouseId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}