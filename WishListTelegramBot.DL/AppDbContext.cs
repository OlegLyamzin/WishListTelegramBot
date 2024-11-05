using WishListTelegramBot.Core;
using WishListTelegramBot.DL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace WishListTelegramBot.DL
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<WishList> WishList { get; set; }
        public DbSet<Wish> Wish { get; set; }


        private string _connectionString;
        public AppDbContext(IOptions<AppSettings> appSettings)
        {
            _connectionString = appSettings.Value.CONNECTION_STRING;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var serverVersion = MySqlServerVersion.AutoDetect(_connectionString);
            optionsBuilder.UseMySql(_connectionString, serverVersion);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Defining the foreign keys and relationships
            modelBuilder.Entity<WishList>()
                .HasOne(wl => wl.User)
                .WithMany(u => u.WishLists)
                .HasForeignKey(wl => wl.UserId);

            modelBuilder.Entity<Wish>()
                .HasOne(w => w.WishList)
                .WithMany(wl => wl.Wishes)
                .HasForeignKey(w => w.WishListId);

            modelBuilder.Entity<Wish>()
                .HasOne(w => w.Giver)
                .WithMany()
                .HasForeignKey(w => w.GiverId);  // Optional, depending on your needs
        }
    }

}
