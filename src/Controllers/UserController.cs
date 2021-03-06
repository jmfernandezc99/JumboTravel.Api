using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Users.Requests;
using JumboTravel.Api.src.Domain.Models.Users.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JumboTravel.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Login([FromBody] LoginRequest rq)
        {
            try
            {
                var result = await _userService.Login(rq).ConfigureAwait(false);

                return result.Exit == 0 ? Ok(result) : NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in Login, in UserController");
                throw;
            }
        }

        [HttpGet("LoginWithToken")]
        public async Task<IActionResult> LoginWithToken([FromHeader] string authorization)
        {
            try
            {
                var result = await _userService.Login(new LoginRequest() { JsonWebToken = authorization }).ConfigureAwait(false);

                return result.Exit == 0 ? Ok(result) : Ok(result.Message);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in LoginWithToken, in UserController");
                throw;
            }
        }

        [HttpGet("ValidateToken")]
        public IActionResult ValidateToken([FromHeader] string authorization)
        {
            try
            {
                return Ok(JwtExtension.ValidateToken(authorization));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error in ValidateToken, in UserController");
                throw;
            }
        }
    }
}
