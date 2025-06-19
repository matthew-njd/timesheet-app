using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeSheetApp.Api.Data;
using TimeSheetApp.Api.Dtos;
using TimeSheetApp.Api.Enums;

namespace TimeSheetApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController(ApplicationDbContext context, ILogger<UsersController> logger) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<UsersController> _logger = logger;

        [HttpGet] // Route: /api/Users
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
        {
            _logger.LogInformation("Admin user attempting to retrieve all users.");

            var users = await _context.Users
                .Select(u => new UserResponseDto
                {
                    UserId = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive
                })
                .ToListAsync();

            _logger.LogInformation("Successfully retrieved {Count} users.", users.Count);
            return Ok(users);
        }

        [HttpGet("{id}")] // Route: /api/Users/{id}
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponseDto>> GetUserById(int id)
        {
            var authenticatedUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (authenticatedUserIdClaim == null || !int.TryParse(authenticatedUserIdClaim.Value, out int authenticatedUserId))
            {
                _logger.LogWarning("Authenticated user ID not found in token for request to get user {UserId}.", id);
                return Unauthorized("User ID not found in token.");
            }

            bool isAdmin = User.IsInRole(UserRole.Admin.ToString());

            if (authenticatedUserId != id && !isAdmin)
            {
                _logger.LogWarning("User {AuthenticatedUserId} attempted to access user {TargetUserId} without admin privileges.", authenticatedUserId, id);
                return Forbid();
            }

            _logger.LogInformation("Attempting to retrieve user with ID: {UserId} by authenticated user {AuthenticatedUserId}.", id, authenticatedUserId);

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return NotFound($"User with ID {id} not found.");
            }

            _logger.LogInformation("Successfully retrieved user {UserId}.", id);
            return Ok(new UserResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                IsActive = user.IsActive
            });
        }

        [HttpPut("{id}")] // Route: /api/Users/{id}
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto updateDto)
        {
            var authenticatedUserIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (authenticatedUserIdClaim == null || !int.TryParse(authenticatedUserIdClaim.Value, out int authenticatedUserId))
            {
                _logger.LogWarning("Authenticated user ID not found in token for request to update user {UserId}.", id);
                return Unauthorized("User ID not found in token.");
            }

            bool isAdmin = User.IsInRole(UserRole.Admin.ToString());

            if (authenticatedUserId != id && !isAdmin)
            {
                _logger.LogWarning("User {AuthenticatedUserId} attempted to update user {TargetUserId} without admin privileges.", authenticatedUserId, id);
                return Forbid();
            }

            _logger.LogInformation("Attempting to update user with ID: {UserId} by authenticated user {AuthenticatedUserId}.", id, authenticatedUserId);

            var userToUpdate = await _context.Users.FindAsync(id);

            if (userToUpdate == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update.", id);
                return NotFound($"User with ID {id} not found.");
            }

            userToUpdate.FirstName = updateDto.FirstName ?? userToUpdate.FirstName;
            userToUpdate.LastName = updateDto.LastName ?? userToUpdate.LastName;

            if (isAdmin)
            {
                if (updateDto.IsActive.HasValue)
                {
                    userToUpdate.IsActive = updateDto.IsActive.Value;
                    _logger.LogInformation("Admin setting IsActive for user {UserId} to {IsActive}.", id, userToUpdate.IsActive);
                }

                if (updateDto.Role.HasValue)
                {
                    userToUpdate.Role = updateDto.Role.Value;
                    _logger.LogInformation("Admin setting Role for user {UserId} to {Role}.", id, userToUpdate.Role);
                }
            }
            else
            {
                if (updateDto.IsActive.HasValue || updateDto.Role.HasValue)
                {
                    _logger.LogWarning("User {AuthenticatedUserId} attempted to modify restricted fields (IsActive or Role) for user {TargetUserId}.", authenticatedUserId, id);
                    return BadRequest("Non-admin users cannot update 'IsActive' or 'Role' fields.");
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("User with ID {UserId} updated successfully.", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Users.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Concurrency conflict updating user {UserId}.", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during user update. Please try again.");
            }
        }

        [HttpDelete("{id}")] // Route: /api/Users/{id}
        [Authorize(Roles = "Admin")] // Only admins can "delete" - may change to also add managers
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            _logger.LogInformation("Admin user attempting to soft-delete user with ID: {UserId}.", id);

            var userToSoftDelete = await _context.Users.FindAsync(id);

            if (userToSoftDelete == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for soft-delete.", id);
                return NotFound($"User with ID {id} not found.");
            }

            if (!userToSoftDelete.IsActive)
            {
                _logger.LogInformation("User with ID {UserId} is already inactive. No action needed.", id);
                return NoContent();
            }

            userToSoftDelete.IsActive = false;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("User with ID {UserId} soft-deleted successfully (IsActive set to false).", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Users.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Concurrency conflict during soft-delete of user {UserId}.", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error soft-deleting user with ID: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during soft-delete. Please try again.");
            }
        }
    }
}