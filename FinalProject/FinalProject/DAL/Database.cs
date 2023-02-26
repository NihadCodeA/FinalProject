using FinalProject.Models;
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
    }
}
