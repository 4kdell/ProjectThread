using Microsoft.EntityFrameworkCore;

namespace ProjectThread.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Thread> Threads { get; set; }


    }
}
