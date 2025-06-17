using Microsoft.AspNetCore.Mvc;
using TimeSheetApp.Api.Data;
using TimeSheetApp.Api.Models;

namespace TimeSheetApp.Api.Controllers
{
    [ApiController]
    public class UsersController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            var users = context.Users.ToList();

            return users;
        }

        [HttpGet("{id:int}")]
        public ActionResult<User> GetUser(int id)
        {
            var user = context.Users.Find(id);

            if (user == null) return NotFound();

            return user;
        }
    }
}