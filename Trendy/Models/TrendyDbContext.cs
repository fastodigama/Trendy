using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Trendy.Models
{
    public class TrendyDbContext: DbContext
    {

        public DbSet<Topic> Topics { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DbSet<Category> Categories { get; set; }
        public TrendyDbContext (DbContextOptions<TrendyDbContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
