using InterviewMauiBlazor.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Emit;

namespace InterviewMauiBlazor.Database
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var now = DateTime.Now.Date;

            builder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Order>()
                .HasMany(o => o.Transactions)
                .WithOne(t => t.Order)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Transaction>()
                .HasKey(t => new { t.OrderId, t.ProductId });

            builder.Entity<Transaction>()
                .HasOne(t => t.Product)
                .WithMany()
                .HasForeignKey(t => t.ProductId);

            builder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "John Doe", Email = "john@example.com" },
            new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
            );

            builder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Price = 999.99m },
                new Product { Id = 2, Name = "Smartphone", Price = 599.99m },
                new Product { Id = 3, Name = "Ipad", Price = 699.99m }
            );

            builder.Entity<Order>().HasData(               
                new Order { Id = 1, OrderDate = now.AddDays(-6).Date, CustomerId = 1 },
                new Order { Id = 2, OrderDate = now.AddDays(-5).Date, CustomerId = 2 },
                new Order { Id = 3, OrderDate = now.AddDays(-4).Date, CustomerId = 1 },
                new Order { Id = 4, OrderDate = now.AddDays(-3).Date, CustomerId = 2 },
                new Order { Id = 5, OrderDate = now.AddDays(-2).Date, CustomerId = 2 },
                new Order { Id = 6, OrderDate = now.AddDays(-1).Date, CustomerId = 1 },
                new Order { Id = 7, OrderDate = now, CustomerId = 1 },
                new Order { Id = 8, OrderDate = now.AddDays(1), CustomerId = 2 }
            );

            builder.Entity<Transaction>().HasData(
                new Transaction
                {
                    OrderId = 1,
                    ProductId = 2,
                    Quantity = 2,
                    TotalPrice = 1199.98m,
                    Buyer = "Jane Smith",
                    Seller = "TechStore",
                    Time = DateTime.Now.AddDays(-2).Date,
                    Status = "Completed"
                },
                new Transaction
                {
                    OrderId = 2,
                    ProductId = 3,
                    Quantity = 2,
                    TotalPrice = 1399.98m,
                    Buyer = "Jane Smith",
                    Seller = "TechStore",
                    Time = DateTime.Now.AddDays(-1).Date,
                    Status = "Pending"
                },
                new Transaction
                {
                    OrderId = 3,
                    ProductId = 1,
                    Quantity = 1,
                    TotalPrice = 999.99m,
                    Buyer = "John Doe",
                    Seller = "TechStore",
                    Time = DateTime.Now.AddDays(-1).Date,
                    Status = "Completed"
                },
                new Transaction
                {
                    OrderId = 4,
                    ProductId = 2,
                    Quantity = 2,
                    TotalPrice = 1199.98m,
                    Buyer = "Jane Smith",
                    Seller = "TechStore",
                    Time = now,
                    Status = "Pending"
                }
            );
        }
    }
}
