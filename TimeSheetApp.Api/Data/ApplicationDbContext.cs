using Microsoft.EntityFrameworkCore;
using TimeSheetApp.Api.Models;

namespace TimeSheetApp.Api.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}