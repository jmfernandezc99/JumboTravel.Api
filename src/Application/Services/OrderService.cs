using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Orders.Requests;
using JumboTravel.Api.src.Domain.Models.Orders.Responses;

namespace JumboTravel.Api.src.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly DapperContext _context;

        public OrderService(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public Task<CreateOrderResponse?> CreateOrder(CreateOrderRequest rq)
        {
            int decryptedId = EncryptExtension.Decrypt(rq.UserId!);

            using (var connection = _context.CreateConnection())
            {
                string queryCreateOrder = $"INSERT INTO orders()";

                /*
                 INSERT INTO ORDERS (attendant_id , provider_id , date, status)
                    VALUES
	                    (1, 1, '2022-03-18', 'progress');
                 */

                return null;
            }
        }
    }
}
