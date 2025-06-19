using Microsoft.AspNetCore.Mvc;
using TimeSheetApp.Api.Data;
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
        private readonly ILogger<AuthController> _logger = logger; // Recommended for logging
    }
}