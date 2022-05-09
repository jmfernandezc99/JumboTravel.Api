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
        private readonly IPlaneService _planeService;

        public PlaneController(ILogger<PlaneController> logger, IPlaneService planeService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _planeService = planeService ?? throw new ArgumentNullException(nameof(planeService));
        }

        [HttpGet("GetPlaneStock")]
        [ProducesResponseType(typeof(GetPlaneStockResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetPlaneStock([FromHeader] string authorization)
        {
            try
            {
                var result = await _planeService.GetPlaneStock(authorization).ConfigureAwait(false);

                return !string.IsNullOrEmpty(result.Plane) ? Ok(result) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPlainStock, in PlaneController");
                throw;
            }
        }

        [HttpGet("GetPlaneStatus")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetPlaneStatus([FromHeader] string authorization)
        {
            try
            {
                return Ok(await _planeService.GetPlaneStatus(authorization).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPlaneStatus, in PlaneController");
                throw;
            }
        }

        [HttpGet("ChangePlaneStatus")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ChangePlaneStatus([FromHeader] string authorization)
        {
            try
            {
                return Ok(await _planeService.ChangePlaneStatus(authorization).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChangePlaneStatus, in PlaneController");
                throw;
            }
        }
    }
}
