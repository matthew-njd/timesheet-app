using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeSheetApp.Api.Data;
using TimeSheetApp.Api.Dtos;
using TimeSheetApp.Api.Enums;
using TimeSheetApp.Api.Models;
using TimeSheetApp.Api.Services;

namespace TimeSheetApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<AuthController> logger) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly ILogger<AuthController> _logger = logger;

        [HttpPost("register")] // Route: /api/Auth/register
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            _logger.LogInformation("Attempting to register user with email: {Email}", registrationDto.Email);

            if (await _context.Users.AnyAsync(u => u.Email == registrationDto.Email))
            {
                _logger.LogWarning("Registration failed: Email '{Email}' already exists.", registrationDto.Email);
                return Conflict("User with this email already exists.");
            }

            try
            {
                string hashedPassword = _passwordHasher.HashPassword(registrationDto.Password);

                var newUser = new User
                {
                    Email = registrationDto.Email,
                    PasswordHash = hashedPassword,
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    Role = UserRole.User,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User '{Email}' registered successfully with ID: {UserId}", registrationDto.Email, newUser.Id);
                return Ok(new { Message = "User registered successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for email: {Email}", registrationDto.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during registration. Please try again.");
            }
        }

        [HttpPost("login")] // Route: /api/Auth/login
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            _logger.LogInformation("Attempting to log in user with email: {Email}", loginDto.Email);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
            {
                _logger.LogWarning("Login failed for email '{Email}': User not found.", loginDto.Email);
                return Unauthorized("Invalid credentials.");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed for email '{Email}': User is inactive.", loginDto.Email);
                return Unauthorized("Your account is inactive. Please contact support.");
            }

            bool isPasswordValid = _passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed for email '{Email}': Invalid password.", loginDto.Email);
                return Unauthorized("Invalid credentials.");
            }

            try
            {
                string token = _jwtTokenGenerator.GenerateToken(user);

                _logger.LogInformation("User '{Email}' logged in successfully.", loginDto.Email);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT for user: {Email}", loginDto.Email);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during login. Please try again.");
            }
        }
    }
}