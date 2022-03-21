using JumboTravel.Api.src.Domain.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace JumboTravel.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost("GetUser")]
        public async Task<IActionResult> GetUser([FromBody] GetUserRequest rq)
        {
            return Ok(rq);
        }
    }
}
