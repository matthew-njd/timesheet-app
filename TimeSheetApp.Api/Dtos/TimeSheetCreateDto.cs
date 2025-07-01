using System.ComponentModel.DataAnnotations;

namespace TimeSheetApp.Api.Dtos
{
    public class TimeSheetCreateDto
    {
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }
        [Required(ErrorMessage = "End time is required.")]
        public DateTime EndTime { get; set; }
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}