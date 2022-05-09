using Dapper;
using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Planes;
using JumboTravel.Api.src.Domain.Models.PlaneStocks.Responses;
using JumboTravel.Api.src.Domain.Models.Users;

namespace JumboTravel.Api.src.Application.Services
{
    public class PlaneService : IPlaneService
    {
        private readonly DapperContext _context;
        public PlaneService(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> ChangePlaneStatus(string authorization)
        {
            User user = JwtExtension.ReturnUserFromToken(authorization);

            using (var connection = _context.CreateConnection())
            {
                string queryGetUser = $"select * from users where nif = '{user.Nif}'";
                var users = await connection.QueryAsync<User>(queryGetUser).ConfigureAwait(false);

                string queryGetStock = $"select status, p.id as planeid from planes as p inner join attendants as a on a.plane_id = p.id where user_id = { users.FirstOrDefault()!.Id }";

                var queryGetPlaneResponse = await connection.QueryAsync<Plane>(queryGetStock).ConfigureAwait(false);
                int status = queryGetPlaneResponse.FirstOrDefault()!.Status == 0 ? 1 : 0;

                string queryUpdateStatus = $"update planes set status = { status } where id = '{ queryGetPlaneResponse.FirstOrDefault()!.PlaneId }'";

                return await connection.ExecuteAsync(queryUpdateStatus).ConfigureAwait(false) == 1 ? true : false;
            }
        }

        public async Task<int> GetPlaneStatus(string authorization)
        {
            User user = JwtExtension.ReturnUserFromToken(authorization);

            using (var connection = _context.CreateConnection())
            {
                string queryGetUser = $"select * from users where nif = '{user.Nif}'";
                var users = await connection.QueryAsync<User>(queryGetUser).ConfigureAwait(false);

                string queryGetStock = $"select status from planes as p inner join attendants as a on a.plane_id = p.id where user_id = { users.FirstOrDefault()!.Id }";

                var queryGetPlaneResponse = await connection.QueryAsync<Plane>(queryGetStock).ConfigureAwait(false);
                return queryGetPlaneResponse.FirstOrDefault()!.Status;
            }
        }

        public async Task<GetPlaneStockResponse> GetPlaneStock(string authorization)
        {
            User user = JwtExtension.ReturnUserFromToken(authorization);

            using (var connection = _context.CreateConnection())
            {
                string queryGetUser = $"select * from users where nif = '{user.Nif}'";
                var users = await connection.QueryAsync<User>(queryGetUser).ConfigureAwait(false);

                string queryGetStock = $"select P.name as ProductName, PS.quantity as Quantity from attendants as A inner join planes as PL " +
                    $"on A.plane_id = PL.id inner join planestock as PS on PL.id = PS.plane_id inner join products as P " +
                    $"on PS.product_id = P.id where A.user_id = {users.FirstOrDefault()!.Id}";

                string queryGetPlane = $"select P.name, P.status from Planes as P inner join attendants as A on A.plane_id = P.id where A.user_id = { users.FirstOrDefault()!.Id }";

                var queryGetPlaneResponse = await connection.QueryAsync<Plane>(queryGetPlane).ConfigureAwait(false);
                var queryGetStockResponse = await connection.QueryAsync<PlaneStockResponse>(queryGetStock).ConfigureAwait(false);
                return !string.IsNullOrEmpty(queryGetPlaneResponse.FirstOrDefault()!.Name) ? new GetPlaneStockResponse()
                {
                    Plane = queryGetPlaneResponse.FirstOrDefault()!.Name!,
                    Status = queryGetPlaneResponse.FirstOrDefault()!.Status,
                    Stock = queryGetStockResponse.ToList()
                } : new GetPlaneStockResponse();
            }
        }
    }
}
