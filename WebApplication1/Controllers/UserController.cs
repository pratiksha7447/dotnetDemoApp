using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserController:ControllerBase
    {
        private readonly EmployeeContext _userContext;
        public UserController(EmployeeContext userContext)
        {
            _userContext = userContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> Get()
        {
            var users = await _userContext.Users.ToListAsync();
            if (users==null)
            {
                return NotFound();
            }
            return Ok(users);

        }
    }
}
