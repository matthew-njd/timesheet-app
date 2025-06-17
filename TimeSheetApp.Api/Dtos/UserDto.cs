namespace TimeSheetApp.Api.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}