using Microsoft.EntityFrameworkCore;
using TimeSheetApp.Api.Models;

namespace TimeSheetApp.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        
        public DbSet<User> Users { get; set; }
    }
}