using System.Security.Claims;
using TimeSheetApp.Api.Models;

namespace TimeSheetApp.Api.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, IEnumerable<Claim>? additionalClaims = null);
    }
}   