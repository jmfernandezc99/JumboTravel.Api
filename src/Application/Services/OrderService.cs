﻿using Dapper;
using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Attendants;
using JumboTravel.Api.src.Domain.Models.OrderLines;
using JumboTravel.Api.src.Domain.Models.OrderLines.Responses;
using JumboTravel.Api.src.Domain.Models.Orders;
using JumboTravel.Api.src.Domain.Models.Orders.Requests;
using JumboTravel.Api.src.Domain.Models.Orders.Responses;
using JumboTravel.Api.src.Domain.Models.PlaneStocks;
using JumboTravel.Api.src.Domain.Models.Products;
using JumboTravel.Api.src.Domain.Models.Users;

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

        public async Task<List<Order>> GetOrdersByBase(string location)
        {
            using (var connection = _context.CreateConnection())
            {
                string getOrdersQuery = $"select O.id, O.base, O.date, O.status, P.name as plane from orders as O inner join attendants as A ON O.attendant_id = A.id " +
                    $"inner join Planes as P ON A.plane_id = P.id where O.base = '{location}' and O.status = 'progress'";
                var getOrdersResponse = await connection.QueryAsync<Order>(getOrdersQuery).ConfigureAwait(false);

                return getOrdersResponse.Count() > 0 ? getOrdersResponse.ToList() : new List<Order>();
            }
        }

        public async Task<bool> CompleteOrder(CompleteOrderRequest rq)
        {
            using (var connection = _context.CreateConnection())
            {
                string getPlaneIdQuery = $"select A.plane_id as PlaneId from attendants as A inner join orders as O on A.id = O.attendant_id where O.id = {rq.OrderId}";
                var getPlaneIdResponse = await connection.QueryAsync<Attendant>(getPlaneIdQuery).ConfigureAwait(false);

                int planeId = getPlaneIdResponse.ToList()[0].PlaneId;

                string getOrderLinesQuery = $"select id, product_id as ProductId, order_id as OrderId, quantity from orderlines where order_id = {rq.OrderId}";
                var getOrderLinesResponse = await connection.QueryAsync<OrderLine>(getOrderLinesQuery).ConfigureAwait(false);
                var orderLines = getOrderLinesResponse.ToList();

                foreach (var item in orderLines)
                {
                    string getPlaneStockQuantityQuery = $"select * from planestock where product_id = {item.ProductId} and plane_id = {planeId}";
                    var getPlaneStockQuantityResponse = await connection.QueryAsync<PlaneStock>(getPlaneStockQuantityQuery).ConfigureAwait(false);

                    int upgradedStock = getPlaneStockQuantityResponse.ToList()[0].Quantity + item.Quantity;

                    string updateQuery = $"UPDATE planestock SET quantity = {upgradedStock} WHERE product_id = {item.ProductId} and plane_id = {planeId}; ";

                    await connection.ExecuteAsync(updateQuery).ConfigureAwait(false);
                }

                string getAttendantIdQuery = $"select U.id from users as U inner join attendants as A on U.id = A.user_id inner join orders as O on A.id = O.attendant_id where O.id = {rq.OrderId}";
                var getAttendantIdResponse = await connection.QueryAsync<User>(getAttendantIdQuery).ConfigureAwait(false);

                int attendantId = getAttendantIdResponse.ToList()[0].Id;

                string createNotificationQuery = $"INSERT INTO NOTIFICATIONS (user_id, title, description) VALUES ({attendantId}, 'Recarga avión', 'Su avión ha sido recargado')";
                await connection.ExecuteAsync(createNotificationQuery).ConfigureAwait(false);

                string updateOrderToCompleted = $"UPDATE orders SET status = 'completed' where attendant_id = {attendantId} and status = 'progress'";
                await connection.ExecuteAsync(updateOrderToCompleted).ConfigureAwait(false);

                return true;
            }
        }

        public async Task<List<Order>> GetAllOrdersByBase(string location)
        {
            using (var connection = _context.CreateConnection())
            {
                string getOrdersQuery = $"select O.id, O.base, O.date, O.status, P.name as plane from orders as O inner join attendants as A ON O.attendant_id = A.id " +
                    $"inner join Planes as P ON A.plane_id = P.id where O.base = '{location}'";
                var getOrdersResponse = await connection.QueryAsync<Order>(getOrdersQuery).ConfigureAwait(false);

                return getOrdersResponse.Count() > 0 ? getOrdersResponse.ToList() : new List<Order>();
            }
        }
    }
}
