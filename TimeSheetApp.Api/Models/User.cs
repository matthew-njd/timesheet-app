using System.ComponentModel.DataAnnotations;
using TimeSheetApp.Api.Enums;

namespace TimeSheetApp.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        [StringLength(100)]
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }
        public required UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<TimeSheet> TimeSheets { get; set; } = [];
    }
}