namespace TimeSheetApp.Api.Services
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 10;

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(password, WorkFactor);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
        }
    }
}