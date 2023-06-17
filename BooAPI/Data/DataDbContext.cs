using BooAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookAPI.Data
{
    public class DataDbContext:IdentityDbContext<AppUser>
    {
        public DataDbContext(DbContextOptions<DataDbContext> options):base(options)
        {
            
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}
