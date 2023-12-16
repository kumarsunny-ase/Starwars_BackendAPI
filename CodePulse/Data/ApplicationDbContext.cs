using System;
using CodePulse.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Search> searchs { get; set; }
        public DbSet<User> users { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>().ToTable("users");
        //}
    }
}

