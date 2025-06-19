using System.ComponentModel.DataAnnotations;
using TimeSheetApp.Api.Enums;

namespace TimeSheetApp.Api.Dtos
{
    public class UserUpdateDto
    {
        [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
        public string? LastName { get; set; }

        public bool? IsActive { get; set; }

        public UserRole? Role { get; set; }
    }
}