﻿using Dapper;
using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Interfaces.Services;
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
        public async Task<List<PlaneStockResponse>> GetPlaneStock(string authorization)
        {
            User user = JwtExtension.ReturnUserFromToken(authorization);

            using (var connection = _context.CreateConnection())
            {
                string queryGetUser = $"select * from users where nif = '{user.Nif}'";
                var users = await connection.QueryAsync<User>(queryGetUser).ConfigureAwait(false);

                string queryGetStock = $"select P.name as ProductName, PS.quantity as Quantity from attendants as A inner join planes as PL " +
                    $"on A.plane_id = PL.id inner join planestock as PS on PL.id = PS.plane_id inner join products as P " +
                    $"on PS.product_id = P.id where A.user_id = {users.FirstOrDefault()!.Id}";

                var queryGetStockResponse = await connection.QueryAsync<PlaneStockResponse>(queryGetStock).ConfigureAwait(false);
                return queryGetStockResponse.ToList();
            }
        }
    }
}
