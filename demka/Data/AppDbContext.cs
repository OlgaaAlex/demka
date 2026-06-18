// demka/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using demka.Models;

namespace demka
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=torg;Username=postgres;Password=1234567890");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasKey(x => x.ProductArticul);
            modelBuilder.Entity<Product>().Property(x => x.ProductPrice)
                         .HasColumnType("numeric(10,2)");

            // Сопоставление таблиц
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<Role>().ToTable("role");
            modelBuilder.Entity<Category>().ToTable("category");
            modelBuilder.Entity<Manufacturer>().ToTable("manufacturer");
            modelBuilder.Entity<Supplier>().ToTable("supplier");
            modelBuilder.Entity<Status>().ToTable("status");
            modelBuilder.Entity<Address>().ToTable("address");
            modelBuilder.Entity<Order>().ToTable("order");
            modelBuilder.Entity<OrderProduct>().ToTable("orderproduct");
            modelBuilder.Entity<Product>().ToTable("product");

            // ==================== User ====================
            modelBuilder.Entity<User>().Property(u => u.Userid).HasColumnName("userid");
            modelBuilder.Entity<User>().Property(u => u.Role).HasColumnName("role");
            modelBuilder.Entity<User>().Property(u => u.Login).HasColumnName("login");
            modelBuilder.Entity<User>().Property(u => u.Password).HasColumnName("password");
            modelBuilder.Entity<User>().Property(u => u.FirstName).HasColumnName("firstname");
            modelBuilder.Entity<User>().Property(u => u.LastName).HasColumnName("lastname");
            modelBuilder.Entity<User>().Property(u => u.MiddleName).HasColumnName("middlename");

            modelBuilder.Entity<User>()
                .HasOne(u => u.RoleNavigation)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.Role)
                .HasPrincipalKey(r => r.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Role>().Property(r => r.RoleId).HasColumnName("roleid");
            modelBuilder.Entity<Role>().Property(r => r.RoleName).HasColumnName("rolename");

            // ==================== Product ====================
            modelBuilder.Entity<Product>().Property(p => p.ProductArticul).HasColumnName("productarticul");
            modelBuilder.Entity<Product>().Property(p => p.ProductName).HasColumnName("productname");
            modelBuilder.Entity<Product>().Property(p => p.ProductUnit).HasColumnName("productunit");
            modelBuilder.Entity<Product>().Property(p => p.ProductPrice).HasColumnName("productprice");
            modelBuilder.Entity<Product>().Property(p => p.Supplier).HasColumnName("supplier");
            modelBuilder.Entity<Product>().Property(p => p.Manufacturer).HasColumnName("manufacturer");
            modelBuilder.Entity<Product>().Property(p => p.Category).HasColumnName("category");
            modelBuilder.Entity<Product>().Property(p => p.Discount).HasColumnName("discount");
            modelBuilder.Entity<Product>().Property(p => p.CountInStock).HasColumnName("countinstock");
            modelBuilder.Entity<Product>().Property(p => p.Description).HasColumnName("description");
            modelBuilder.Entity<Product>().Property(p => p.PhotoPath).HasColumnName("photopath");

            // Связи Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.CategoryNavigation)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.Category)
                .HasPrincipalKey(c => c.CategoryId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.ManufacturerNavigation)
                .WithMany(m => m.Products)
                .HasForeignKey(p => p.Manufacturer)
                .HasPrincipalKey(m => m.ManufacturerId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.SupplierNavigation)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.Supplier)
                .HasPrincipalKey(s => s.SupplierId);

            // ==================== Category, Manufacturer, Supplier ====================
            modelBuilder.Entity<Category>().Property(c => c.CategoryId).HasColumnName("categoryid");
            modelBuilder.Entity<Category>().Property(c => c.CategoryName).HasColumnName("categoryname");

            modelBuilder.Entity<Manufacturer>().Property(m => m.ManufacturerId).HasColumnName("manufacturerid");
            modelBuilder.Entity<Manufacturer>().Property(m => m.ManufacturerName).HasColumnName("manufacturername");

            modelBuilder.Entity<Supplier>().Property(s => s.SupplierId).HasColumnName("supplierid");
            modelBuilder.Entity<Supplier>().Property(s => s.SupplierName).HasColumnName("suppliername");

            // ==================== Order ====================
            modelBuilder.Entity<Order>().Property(o => o.OrderId).HasColumnName("orderid");
            modelBuilder.Entity<Order>().Property(o => o.OrderStatus).HasColumnName("orderstatus");
            modelBuilder.Entity<Order>().Property(o => o.OrderAddress).HasColumnName("orderaddress");
            modelBuilder.Entity<Order>().Property(o => o.OrderDate).HasColumnName("orderdate");
            modelBuilder.Entity<Order>().Property(o => o.OrderDateIssue).HasColumnName("orderdateissue");
            modelBuilder.Entity<Order>().Property(o => o.OrderUser).HasColumnName("orderuser");
            modelBuilder.Entity<Order>().Property(o => o.OrderCode).HasColumnName("ordercode");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.StatusNavigation)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.OrderStatus)
                .HasPrincipalKey(s => s.StatusId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.AddressNavigation)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.OrderAddress)
                .HasPrincipalKey(a => a.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.UserNavigation)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.OrderUser)
                .HasPrincipalKey(u => u.Userid)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Status>().Property(s => s.StatusId).HasColumnName("statusid");
            modelBuilder.Entity<Status>().Property(s => s.StatusName).HasColumnName("statusname");

            modelBuilder.Entity<Address>().Property(a => a.AddressId).HasColumnName("addressid");
            modelBuilder.Entity<Address>().Property(a => a.AddressName).HasColumnName("addressname");

            // ==================== OrderProduct ====================
            modelBuilder.Entity<OrderProduct>().ToTable("orderproduct");

            modelBuilder.Entity<OrderProduct>()
                .Property(op => op.OrderProductId).HasColumnName("orderproductid");

            modelBuilder.Entity<OrderProduct>()
    .Property(op => op.OrderId)
    .HasColumnName("ORDER");        

            modelBuilder.Entity<OrderProduct>()
                .Property(op => op.Product).HasColumnName("product");

            modelBuilder.Entity<OrderProduct>()
                .Property(op => op.OrderProductCount).HasColumnName("orderproductcount");

            // Связи
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.OrderNavigation)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(op => op.OrderId)           // EF будет использовать свойство OrderId
                .HasPrincipalKey(o => o.OrderId);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.ProductNavigation)
                .WithMany(p => p.OrderProducts)
                .HasForeignKey(op => op.Product)
                .HasPrincipalKey(p => p.ProductArticul);
        }
    }
}