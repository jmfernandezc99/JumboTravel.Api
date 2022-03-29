using JumboTravel.Api.src.Domain.Models.Orders.Requests;
using JumboTravel.Api.src.Domain.Models.Orders.Responses;

namespace JumboTravel.Api.src.Domain.Interfaces.Services
{
    public interface IOrderService
    {
        Task<CreateOrderResponse?> CreateOrder(CreateOrderRequest rq);
    }
}
