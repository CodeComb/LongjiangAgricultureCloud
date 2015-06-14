using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace LongjiangAgricultureCloud.Models
{
    public class LongjiangAgricultureCloudContext : DbContext
    {
        public DbSet<Area> Areas { get; set; }

        public DbSet<Catalog> Catalogs { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Information> Informations { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Provider> Providers { get; set; }

        public DbSet<User> Users { get; set; }
         
        public DbSet<Store> Stores { get; set; }

        public LongjiangAgricultureCloudContext() : base("mssqldb") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Providers)
                .WithOptional(p => p.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Stores)
                .WithOptional(s => s.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Area>()
                .HasMany(a => a.Users)
                .WithOptional(u => u.Area)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Provider>()
                .HasMany(p => p.Products)
                .WithOptional(p=>p.Provider)
                .WillCascadeOnDelete(false);
        }
    }
}