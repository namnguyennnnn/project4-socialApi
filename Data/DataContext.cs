using DoAn4.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn4.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
        
        }

        public DbSet<User> Users { get; set; }
    }
}
