using JumboTravel.Api.src.Domain.Models.OrderLines.Responses;
using JumboTravel.Api.src.Domain.Models.Orders;
using JumboTravel.Api.src.Domain.Models.Orders.Requests;
using JumboTravel.Api.src.Domain.Models.Orders.Responses;

namespace JumboTravel.Api.src.Domain.Interfaces.Services
{
    public interface IOrderService
    {
        Task<CreateOrderResponse?> CreateOrder(CreateOrderRequest rq);
        Task<bool> CompleteOrder(CompleteOrderRequest rq);
        Task<List<Order>> GetOrders(string userId);
        Task<ObtainInvoiceResponse?> ObtainInvoice(int orderId);
        Task<List<Order>> GetOrdersByBase(string location);
        Task<List<Order>> GetAllOrdersByBase(string location);
        Task<bool> CanCreateOrder(string userId);
        Task<List<GetOrderLinesResponse>> GetOrderLinesByOrderId(string orderId);
    }
}
