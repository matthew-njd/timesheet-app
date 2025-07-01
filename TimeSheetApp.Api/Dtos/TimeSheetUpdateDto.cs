using System.ComponentModel.DataAnnotations;
using TimeSheetApp.Api.Enums;

namespace TimeSheetApp.Api.Dtos
{
    public class TimeSheetUpdateDto
    {
        public DateTime? Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        public TimeSheetStatus? Status { get; set; }
    }
}