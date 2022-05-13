using Dapper;
using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Attendants;
using JumboTravel.Api.src.Domain.Models.OrderLines;
using JumboTravel.Api.src.Domain.Models.OrderLines.Responses;
using JumboTravel.Api.src.Domain.Models.Orders;
using JumboTravel.Api.src.Domain.Models.Orders.Requests;
using JumboTravel.Api.src.Domain.Models.Orders.Responses;
using JumboTravel.Api.src.Domain.Models.Planes;
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
        public async Task<CreateOrderResponse?> CreateOrder(CreateOrderRequest rq, string token)
        {
            User user = JwtExtension.ReturnUserFromToken(token);

            using var connection = _context.CreateConnection();
            string queryGetUser = $"select * from users where nif = '{user.Nif}'";
            var users = await connection.QueryAsync<User>(queryGetUser).ConfigureAwait(false);

            DateTime now = DateTime.Now.Date;
            string date = now.ToString("yyyy-MM-dd");

            string getPlaneIdQuery = $"SELECT plane_id as planeId FROM attendants where id = { users.FirstOrDefault()!.Id }";
            var planeIdResponse = await connection.QueryAsync<Attendant>(getPlaneIdQuery).ConfigureAwait(false);
            int planeId = planeIdResponse.FirstOrDefault()!.PlaneId;

            string queryCreateOrder = $"INSERT INTO orders(attendant_id, base, date, status, plane_id) " +
                $"VALUES ({ users.FirstOrDefault()!.Id }, '{ rq.Base }', '{ date }', 'progress', { planeId })";

            var createOrderResponse = await connection.ExecuteAsync(queryCreateOrder, new Order()).ConfigureAwait(false);

            string queryGetOrder = $"select id from orders where date = '{date}' and attendant_id = {users.FirstOrDefault()!.Id}";

            var getOrderQueryResponse = await connection.QueryAsync<Order>(queryGetOrder).ConfigureAwait(false);

            var order = getOrderQueryResponse.LastOrDefault();

            string queryCreateOrderLines = "INSERT INTO ORDERLINES (product_id, order_id , quantity) VALUES ";

            for (int i = 0; i < rq.Properties!.Count; i++)
            {
                string queryProductId = $"SELECT id from products where name = '{rq.Properties[i].ProductName}'";
                var getProductIdResponse = await connection.QueryAsync<Product>(queryProductId).ConfigureAwait(false);
                var product = getProductIdResponse.FirstOrDefault();

                string queryMaxStockPerProduct = $"select maximum_quantity as MaximumQuantity, quantity from planestock where product_id = {product!.Id} and plane_id = { planeId }";
                var getMaxStockPerProduct = await connection.QueryAsync<PlaneStock>(queryMaxStockPerProduct).ConfigureAwait(false);
                int maxStock = getMaxStockPerProduct.FirstOrDefault()!.MaximumQuantity;
                int quantity = getMaxStockPerProduct.FirstOrDefault()!.Quantity;

                if (rq.Properties[i].Quantity + quantity > maxStock)
                {
                    string deleteLastOrder = $"delete from Orders where id = {order!.Id}";
                    await connection.ExecuteAsync(deleteLastOrder).ConfigureAwait(false);
                    return new CreateOrderResponse() { OutOfRange = true };
                }

                queryCreateOrderLines += $"({product!.Id}, {order!.Id}, {rq.Properties[i].Quantity})";
                queryCreateOrderLines += (i + 1) == rq.Properties.Count ? ";" : ",";
            }

            var createOrderLines = await connection.ExecuteAsync(queryCreateOrderLines).ConfigureAwait(false);

            return createOrderLines > 0 ? new CreateOrderResponse() { IsCreated = true, OrderId = order!.Id, OutOfRange = false } : null;
        }

        public async Task<List<Order>> GetOrders(string token)
        {
            User user = JwtExtension.ReturnUserFromToken(token);

            using var connection = _context.CreateConnection();
            string queryGetUser = $"select * from users where nif = '{user.Nif}'";
            var users = await connection.QueryAsync<User>(queryGetUser).ConfigureAwait(false);

            string getOrdersQuery = $"select id, base, date, status from orders where attendant_id = {users.FirstOrDefault()!.Id}";
            var getOrdersResponse = await connection.QueryAsync<Order>(getOrdersQuery).ConfigureAwait(false);

            return getOrdersResponse.Count() > 0 ? getOrdersResponse.ToList() : new List<Order>();
        }

        public async Task<List<GetOrderLinesResponse>> GetOrderLinesByOrderId(string orderId, string token)
        {
            using var connection = _context.CreateConnection();
            string getOrdersLinesQuery = $"select P.name as ProductName, OL.quantity from products as P inner join orderlines as OL on P.id = OL.product_id where order_id = {orderId}";
            var getOrderLinesResponse = await connection.QueryAsync<GetOrderLinesResponse>(getOrdersLinesQuery).ConfigureAwait(false);

            return getOrderLinesResponse.Count() > 0 ? getOrderLinesResponse.ToList() : new List<GetOrderLinesResponse>();
        }

        public async Task<bool> CanCreateOrder(string token)
        {
            User user = JwtExtension.ReturnUserFromToken(token);

            using var connection = _context.CreateConnection();
            string queryGetUser = $"select * from users where nif = '{user.Nif}'";
            var users = await connection.QueryAsync<User>(queryGetUser).ConfigureAwait(false);

            string getPlaneStatusQuery = $"select P.status from planes as P inner join attendants as A on P.id = A.plane_id where A.id = { users.FirstOrDefault()!.Id }";
            var getPlaneStatusResponse = await connection.QueryAsync<Plane>(getPlaneStatusQuery).ConfigureAwait(false);

            int planeStatus = getPlaneStatusResponse.ToList()[0].Status;

            if (planeStatus == 0)
            {
                return false;
            }

            string getPlaneIdQuery = $"SELECT plane_id as planeId FROM attendants where id = { users.FirstOrDefault()!.Id }";
            var planeIdResponse = await connection.QueryAsync<Attendant>(getPlaneIdQuery).ConfigureAwait(false);
            int planeId = planeIdResponse.FirstOrDefault()!.PlaneId;

            string getOrdersQuery = $"select status from orders where plane_id = { planeId }";
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

        public async Task<List<Order>> GetOrdersByBase(string location, string token)
        {
            using var connection = _context.CreateConnection();
            User user = JwtExtension.ReturnUserFromToken(token);
            string queryGetUser = $"select * from users where nif = '{user.Nif}'";
            var users = await connection.QueryAsync<User>(queryGetUser).ConfigureAwait(false);

            if (users.ToList().Count() < 1)
            {
                return new List<Order>();
            }

            string getOrdersQuery = $"select O.id, O.base, O.date, O.status, P.name as plane from orders as O inner join attendants as A ON O.attendant_id = A.id " +
                $"inner join Planes as P ON A.plane_id = P.id where O.base = '{location}' and O.status = 'progress'";
            var getOrdersResponse = await connection.QueryAsync<Order>(getOrdersQuery).ConfigureAwait(false);

            return getOrdersResponse.Count() > 0 ? getOrdersResponse.ToList() : new List<Order>();
        }

        public async Task<bool> CompleteOrder(CompleteOrderRequest rq, string token)
        {
            User user = JwtExtension.ReturnUserFromToken(token);

            using var connection = _context.CreateConnection();
            string queryGetUser = $"select * from users where nif = '{user.Nif}'";
            var users = await connection.QueryAsync<User>(queryGetUser).ConfigureAwait(false);

            if (users.ToList().Count < 1)
            {
                return false;
            }

            string getPlaneIdQuery = $"select A.plane_id as planeId, P.status from attendants as A inner join orders as O on A.id = O.attendant_id inner join Planes as P on P.id = A.plane_id where O.id = {rq.OrderId}";
            var getPlaneIdResponse = await connection.QueryAsync<Plane>(getPlaneIdQuery).ConfigureAwait(false);

            if (getPlaneIdResponse.ToList()[0].Status == 1)
            {
                return false;
            }

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

            string getProviderIdQuery = $"SELECT id from providers where user_id = { users.FirstOrDefault()!.Id }";
            var getProviderIdResponse = await connection.QueryAsync<int>(getProviderIdQuery).ConfigureAwait(false);
            int providerId = getProviderIdResponse.FirstOrDefault()!;

            string updateOrderToCompleted = $"UPDATE orders SET status = 'completed', provider_id = { providerId } where attendant_id = {attendantId} and status = 'progress' and id = {rq.OrderId}";
            await connection.ExecuteAsync(updateOrderToCompleted).ConfigureAwait(false);

            return true;
        }

        public async Task<List<Order>> GetAllOrdersByBase(string location, string token)
        {
            using var connection = _context.CreateConnection();
            User user = JwtExtension.ReturnUserFromToken(token);
            string queryGetUser = $"select * from users where nif = '{user.Nif}'";
            var users = await connection.QueryAsync<User>(queryGetUser).ConfigureAwait(false);

            if (users.ToList().Count() < 1)
            {
                return new List<Order>();
            }

            string getOrdersQuery = $"select O.id, O.base, O.date, O.status, P.name as plane from orders as O inner join attendants as A ON O.attendant_id = A.id " +
                $"inner join Planes as P ON A.plane_id = P.id where O.base = '{location}'";
            var getOrdersResponse = await connection.QueryAsync<Order>(getOrdersQuery).ConfigureAwait(false);

            return getOrdersResponse.Count() > 0 ? getOrdersResponse.ToList() : new List<Order>();
        }

        public async Task<ObtainInvoiceResponse?> ObtainInvoice(ObtainInvoiceRequest rq, string authorization)
        {
            using var connection = _context.CreateConnection();
            User user = JwtExtension.ReturnUserFromToken(authorization);
            string queryGetUser = $"select * from users where nif = '{user.Nif}'";
            var users = await connection.QueryAsync<User>(queryGetUser).ConfigureAwait(false);

            if (!users.ToList().Any())
            {
                return new ObtainInvoiceResponse() { Message = "Not found user with token" };
            }

            string getProductsQuery = $"select P.name as ProductName, OL.quantity, P.price from Products as P inner join OrderLines as OL ON P.id = OL.product_id inner join Orders as O on OL.order_id = O.id where O.date = '{rq.Date}' and O.base = '{rq.Base}'";
            var getProductsResponse = await connection.QueryAsync<ObtainInvoicePropertiesResponse>(getProductsQuery).ConfigureAwait(false);
            var productsDetails = getProductsResponse.ToList();

            decimal totalPrice = 0;

            foreach (var item in productsDetails)
            {
                item.PricePerUnit = item.Price;
                item.Price *= item.Quantity;
                totalPrice += item.Price;
            }

            return new ObtainInvoiceResponse() { Properties = productsDetails, TotalPrice = totalPrice };
        }

        public async Task<List<GetAllOrdersRegistryResponse>> GetOrdersRegistry(string authorization)
        {
            using var connection = _context.CreateConnection();

            User user = JwtExtension.ReturnUserFromToken(authorization);

            string getOrdersQuery = "select attendant_id as attendantId, provider_id as providerId, date, plane_id as plane, status from orders";
            var getOrdersResponse = await connection.QueryAsync<Order>(getOrdersQuery).ConfigureAwait(false);
            var orders = getOrdersResponse.ToList();

            List<GetAllOrdersRegistryResponse> response = new List<GetAllOrdersRegistryResponse>();
            foreach (var order in orders)
            {
                string getAttendantNameQuery = $"select name from users as u inner join attendants as a on u.id = a.user_id where a.id = { order.AttendantId }";
                var getAttendantNameResponse = await connection.QueryAsync<User>(getAttendantNameQuery).ConfigureAwait(false);
                string attendantName = getAttendantNameResponse.FirstOrDefault()!.Name!;

                string getProviderNameQuery = $"select name from users as u inner join providers as p on u.id = p.user_id where p.id = { order.ProviderId }";
                var getProviderNameResponse = await connection.QueryAsync<User>(getProviderNameQuery).ConfigureAwait(false);
                string providerName = getProviderNameResponse.Any() ? getProviderNameResponse.FirstOrDefault()!.Name! : string.Empty;

                string getPlaneNameQuery = $"select name from planes where id = { order.Plane }";
                var getPlaneNameResponse = await connection.QueryAsync<Plane>(getPlaneNameQuery).ConfigureAwait(false);
                string planeName = getPlaneNameResponse.FirstOrDefault()!.Name!;

                response.Add(new GetAllOrdersRegistryResponse()
                {
                    Attendant = attendantName,
                    Date = order.Date!,
                    Plane = planeName,
                    Provider = providerName,
                    Status = order.Status
                });
            }

            return response;
        }
    }
}
