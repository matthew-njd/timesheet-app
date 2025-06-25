using System.ComponentModel.DataAnnotations;
using TimeSheetApp.Api.Enums;

namespace TimeSheetApp.Api.Models
{
    public class TimeSheet
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required DateTime Date { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public TimeSheetStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public User User { get; set; } = null!;
    }
}