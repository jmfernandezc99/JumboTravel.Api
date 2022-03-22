using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace JumboTravel.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost("UserExists")]
        public bool UserExists([FromBody] GetUserRequest rq)
        {
            try
            {
                var result =  _userService.UserExists(rq);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUser, in UserController");
                throw;
            }
        }
    }
}
