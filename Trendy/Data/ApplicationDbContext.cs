using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Trendy.Models;

namespace Trendy.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        // db tables
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<CategoryTopic> CategoryTopics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // needed for Identity tables (don't remove)
            base.OnModelCreating(modelBuilder);

            // setting up composite key for CategoryTopic
            modelBuilder.Entity<CategoryTopic>()
                .HasKey(ct => new { ct.CategoryId, ct.TopicId });

            // link CategoryTopic to Category (many-to-one)
            modelBuilder.Entity<CategoryTopic>()
                .HasOne(ct => ct.Category)
                .WithMany(c => c.CategoryTopics)
                .HasForeignKey(ct => ct.CategoryId);

            // link CategoryTopic to Topic (many-to-one)
            modelBuilder.Entity<CategoryTopic>()
                .HasOne(ct => ct.Topic)
                .WithMany(t => t.CategoryTopics)
                .HasForeignKey(ct => ct.TopicId);
        }


    }
}
