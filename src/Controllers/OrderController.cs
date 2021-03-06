using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.OrderLines;
using JumboTravel.Api.src.Domain.Models.Orders;
using JumboTravel.Api.src.Domain.Models.Orders.Requests;
using JumboTravel.Api.src.Domain.Models.Orders.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JumboTravel.Api.src.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpPost("ObtainInvoice")]
        [ProducesResponseType(typeof(ObtainInvoiceResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObtainInvoice([FromHeader] string authorization, [FromBody] ObtainInvoiceRequest request)
        {
            var result = await _orderService.ObtainInvoice(request, authorization).ConfigureAwait(false);

            if (result == null)
            {
                return BadRequest("Bad Request");
            }

            return Ok(result);
        }

        [HttpPost("CreateOrder")]
        [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrder([FromHeader] string authorization, [FromBody] CreateOrderRequest rq)
        {
            try
            {
                var result = await _orderService.CreateOrder(rq, authorization).ConfigureAwait(false);

                if (result == null)
                {
                    return BadRequest("Bad Request");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateOrder, in OrderController");
                throw;
            }
        }

        [HttpPost("CompleteOrder")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> CompleteOrder([FromHeader] string authorization, [FromBody] CompleteOrderRequest rq)
        {
            try
            {
                var result = await _orderService.CompleteOrder(rq, authorization).ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CompleteOrder, in OrderController");
                throw;
            }
        }

        [HttpGet("CanCreateOrder")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> CanCreateOrder([FromHeader] string authorization)
        {
            try
            {
                var result = await _orderService.CanCreateOrder(authorization).ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrders, in OrderController");
                throw;
            }
        }

        [HttpGet("GetAllOrdersByBase")]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAllOrdersByBase([FromHeader] string authorization, [FromQuery] string location)
        {
            try
            {
                var result = await _orderService.GetAllOrdersByBase(location, authorization).ConfigureAwait(false);

                if (result.Count < 1)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllOrdersByBase, in OrderController");
                throw;
            }
        }

        [HttpGet("GetOrdersByBase")]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetOrdersByBase([FromHeader] string authorization, [FromQuery] string location)
        {
            try
            {
                var result = await _orderService.GetOrdersByBase(location, authorization).ConfigureAwait(false);

                if (result.Count < 1)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrdersByBase, in OrderController");
                throw;
            }
        }

        [HttpGet("GetOrders")]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetOrders([FromHeader] string authorization)
        {
            try
            {
                var result = await _orderService.GetOrders(authorization).ConfigureAwait(false);

                if (result.Count < 1)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrders, in OrderController");
                throw;
            }
        }

        [HttpGet("GetOrderLinesByOrderId")]
        [ProducesResponseType(typeof(List<OrderLine>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetOrderLinesByOrderId([FromHeader] string authorization, [FromQuery] string orderId)
        {
            try
            {
                var result = await _orderService.GetOrderLinesByOrderId(orderId, authorization).ConfigureAwait(false);

                if (result.Count < 1)
                {
                    return NoContent();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrderLinesByOrderId, in OrderController");
                throw;
            }
        }

        [HttpGet("GetOrdersRegistry")]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetOrdersRegistry([FromHeader] string authorization)
        {
            try
            {
                var result = await _orderService.GetOrdersRegistry(authorization).ConfigureAwait(false);

                return result.Count < 1 ? NoContent() : Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrderLinesByOrderId, in OrderController");
                throw;
            }
        }
    }
}
