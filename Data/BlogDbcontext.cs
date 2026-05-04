using Microsoft.EntityFrameworkCore;
using BlogManagementSystem.Models;
using System;
namespace BlogManagementSystem.Data
{
    public class BlogDbcontext:DbContext
    {
        public BlogDbcontext(DbContextOptions<BlogDbcontext> options)   
            : base(options) { }
        public DbSet<Comments> comments { get; set; }
        public DbSet<Posts> posts { get; set; }
        public DbSet<Users> users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comments>()
                .HasOne(c => c.user)
                .WithMany(u => u.comments)
                .HasForeignKey(c => c.usersId)
                .OnDelete(DeleteBehavior.NoAction); 
        }
    }
}
