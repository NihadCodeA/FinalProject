﻿using FinalProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace FinalProject.DAL
{
    public class Database : IdentityDbContext
    {
        public Database(DbContextOptions<Database> options) : base(options) { }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Store> Stores { get; set; } 
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<StoreOrderItem> StoreOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BasketItem>()
                .HasOne(bi => bi.Product)
                .WithMany(p => p.BasketItems)
                .HasForeignKey(bi => bi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
