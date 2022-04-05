using Dapper;
using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.OrderLines.Responses;
using JumboTravel.Api.src.Domain.Models.Orders;
using JumboTravel.Api.src.Domain.Models.Orders.Requests;
using JumboTravel.Api.src.Domain.Models.Orders.Responses;
using JumboTravel.Api.src.Domain.Models.Products;

namespace JumboTravel.Api.src.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly DapperContext _context;

        public OrderService(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<CreateOrderResponse?> CreateOrder(CreateOrderRequest rq)
        {
            int decryptedId = EncryptExtension.Decrypt(rq.UserId!);

            using (var connection = _context.CreateConnection())
            {
                DateTime now = DateTime.Now.Date;
                string date = now.ToString("yyyy-MM-dd");

                string queryCreateOrder = $"INSERT INTO orders(attendant_id, base, date, status) " +
                    $"VALUES ({decryptedId}, '{rq.Base}', '{date}', 'progress')";

                var createOrderResponse = await connection.ExecuteAsync(queryCreateOrder, new Order()).ConfigureAwait(false);

                string queryGetOrder = $"select id from orders where date = '{date}' and attendant_id = {decryptedId}";

                var getOrderQueryResponse = await connection.QueryAsync<Order>(queryGetOrder).ConfigureAwait(false);

                var order = getOrderQueryResponse.LastOrDefault();

                string queryCreateOrderLines = $"INSERT INTO ORDERLINES (product_id, order_id , quantity) VALUES ";

                for (int i = 0; i < rq.Properties!.Count; i++)
                {
                    string queryProductId = $"SELECT id from products where name = '{rq.Properties[i].ProductName}'";
                    var getProductIdResponse = await connection.QueryAsync<Product>(queryProductId).ConfigureAwait(false);
                    var product = getProductIdResponse.FirstOrDefault();

                    queryCreateOrderLines += $"({product!.Id}, {order!.Id}, {rq.Properties[i].Quantity})";

                    queryCreateOrderLines += (i + 1) == rq.Properties.Count ? ";" : ",";
                }

                var createOrderLines = await connection.ExecuteAsync(queryCreateOrderLines).ConfigureAwait(false);

                return createOrderLines > 0 ? new CreateOrderResponse() { IsCreated = true, OrderId = order!.Id } : null;
            }
        }

        public async Task<List<Order>> GetOrders(string userId)
        {
            int decryptedId = EncryptExtension.Decrypt(userId);

            using (var connection = _context.CreateConnection())
            {
                string getOrdersQuery = $"select id, base, date, status from orders where attendant_id = {decryptedId}";
                var getOrdersResponse = await connection.QueryAsync<Order>(getOrdersQuery).ConfigureAwait(false);

                return getOrdersResponse.Count() > 0 ? getOrdersResponse.ToList() : new List<Order>();
            }
        }

        public async Task<List<GetOrderLinesResponse>> GetOrderLinesByOrderId(string orderId)
        {
            using (var connection = _context.CreateConnection())
            {
                string getOrdersLinesQuery = $"select P.name as ProductName, OL.quantity from products as P inner join orderlines as OL on P.id = OL.product_id where order_id = {orderId}";
                var getOrderLinesResponse = await connection.QueryAsync<GetOrderLinesResponse>(getOrdersLinesQuery).ConfigureAwait(false);

                return getOrderLinesResponse.Count() > 0 ? getOrderLinesResponse.ToList() : new List<GetOrderLinesResponse>();
            }
        }

        public async Task<bool> CanCreateOrder(string userId)
        {
            int decryptedId = EncryptExtension.Decrypt(userId);

            using (var connection = _context.CreateConnection())
            {
                string getOrdersQuery = $"select status from orders where attendant_id = {decryptedId}";
                var getOrdersResponse = await connection.QueryAsync<Order>(getOrdersQuery).ConfigureAwait(false);
                bool response = true;

                foreach (var order in getOrdersResponse.ToList())
                {
                    if (order.Status.Equals("progress"))
                    {
                        response = false;
                        break;
                    }
                }

                return response;
            }
        }
    }
}
