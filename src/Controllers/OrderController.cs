﻿using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.OrderLines;
using JumboTravel.Api.src.Domain.Models.Orders;
using JumboTravel.Api.src.Domain.Models.Orders.Requests;
using JumboTravel.Api.src.Domain.Models.Orders.Responses;
using Microsoft.AspNetCore.Mvc;

namespace JumboTravel.Api.src.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpPost("CreateOrder")]
        [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest rq)
        {
            try
            {
                var result = await _orderService.CreateOrder(rq).ConfigureAwait(false);

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

        [HttpGet("CanCreateOrder")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> CanCreateOrder([FromQuery] string userId)
        {
            try
            {
                var result = await _orderService.CanCreateOrder(userId).ConfigureAwait(false);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrders, in OrderController");
                throw;
            }
        }

        [HttpGet("GetOrders")]
        [ProducesResponseType(typeof(List<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetOrders([FromQuery] string userId)
        {
            try
            {
                var result = await _orderService.GetOrders(userId).ConfigureAwait(false);

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
        public async Task<IActionResult> GetOrderLinesByOrderId([FromQuery] string orderId)
        {
            try
            {
                var result = await _orderService.GetOrderLinesByOrderId(orderId).ConfigureAwait(false);

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
    }
}
