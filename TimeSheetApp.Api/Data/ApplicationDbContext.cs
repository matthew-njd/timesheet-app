using Microsoft.EntityFrameworkCore;
using TimeSheetApp.Api.Models;

namespace TimeSheetApp.Api.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<TimeSheet> TimeSheets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUser(modelBuilder);
            ConfigureTimeSheet(modelBuilder);
        }

        private void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).HasConversion<string>();

                entity.HasMany(u => u.TimeSheets)
                    .WithOne(ts => ts.User)
                    .HasForeignKey(ts => ts.UserId);
            });
        }
        
        private void ConfigureTimeSheet(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TimeSheet>(entity =>
            {
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
            });
        }
    }
}