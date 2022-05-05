using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.PlaneStocks.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JumboTravel.Api.src.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PlaneController : ControllerBase
    {
        private readonly ILogger<PlaneController> _logger;
        private readonly IPlaneService _plainService;

        public PlaneController(ILogger<PlaneController> logger, IPlaneService plainService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _plainService = plainService ?? throw new ArgumentNullException(nameof(plainService));
        }

        [HttpGet("GetPlaneStock")]
        [ProducesResponseType(typeof(GetPlaneStockResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetPlaneStock([FromHeader] string authorization)
        {
            try
            {
                var result = await _plainService.GetPlaneStock(authorization).ConfigureAwait(false);

                return !string.IsNullOrEmpty(result.Plane) ? Ok(result) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPlainStock, in PlaneController");
                throw;
            }
        }
    }
}
