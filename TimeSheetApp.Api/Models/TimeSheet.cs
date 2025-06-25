using System.ComponentModel.DataAnnotations;
using TimeSheetApp.Api.Enums;

namespace TimeSheetApp.Api.Models
{
    public class TimeSheet
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public TimeSheetStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User User { get; set; } = null!;
    }
}