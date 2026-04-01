using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrderTracking
{
    public class TrackerContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        //create a protected override void to set the Data Source to CustomerOrders.db
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=CustomerOrders.db");
        }

        //use this protected override method for OnModelCreating
        //Pass in a model builder
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //ensure Customer has a unique email
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(c => c.Email).IsUnique();
            });

            //use the ModelBuilder.Entity to set a one-to-many relationship between customer and orders
            //enforce the foriegn key with HasForeignKey
            //use OnDelete to cascade delete orders for the deleted customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasMany(c => c.Orders)
                      .WithOne(o => o.Customer)
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            //use ModelBuilder.Entity to ensure that Order has a Total Amount
            //set precision to (18,2)
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(o => o.TotalAmount)
                      .HasPrecision(18, 2)
                      .IsRequired();
            });
        }
    }
}
