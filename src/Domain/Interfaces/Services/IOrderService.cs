using JumboTravel.Api.src.Domain.Models.OrderLines.Responses;
using JumboTravel.Api.src.Domain.Models.Orders;
using JumboTravel.Api.src.Domain.Models.Orders.Requests;
using JumboTravel.Api.src.Domain.Models.Orders.Responses;

namespace JumboTravel.Api.src.Domain.Interfaces.Services
{
    public interface IOrderService
    {
        Task<CreateOrderResponse?> CreateOrder(CreateOrderRequest rq, string authorization);
        Task<bool> CompleteOrder(CompleteOrderRequest rq, string authorization);
        Task<List<Order>> GetOrders(string authorization);
        Task<ObtainInvoiceResponse?> ObtainInvoice(ObtainInvoiceRequest rq, string authorization);
        Task<List<Order>> GetOrdersByBase(string location, string authorization);
        Task<List<Order>> GetAllOrdersByBase(string location, string authorization);
        Task<bool> CanCreateOrder(string authorization);
        Task<List<GetOrderLinesResponse>> GetOrderLinesByOrderId(string orderId, string authorization);
    }
}
