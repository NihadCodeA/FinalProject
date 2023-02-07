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
    }
}
