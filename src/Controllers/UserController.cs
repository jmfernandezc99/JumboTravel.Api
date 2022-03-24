using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Users.Requests;
using JumboTravel.Api.src.Domain.Models.Users.Responses;
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

        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Login([FromBody] LoginRequest rq)
        {
            try
            {
                var result = await _userService.Login(rq).ConfigureAwait(false);

                return result != null ? Ok(result) : NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Login, in UserController");
                throw;
            }
        }
    }
}
