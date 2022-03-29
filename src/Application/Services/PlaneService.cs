using Dapper;
using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Attendants;
using JumboTravel.Api.src.Domain.Models.PlaneStocks;
using JumboTravel.Api.src.Domain.Models.PlaneStocks.Requests;
using JumboTravel.Api.src.Domain.Models.PlaneStocks.Responses;
using JumboTravel.Api.src.Domain.Models.Products;

namespace JumboTravel.Api.src.Application.Services
{
    public class PlaneService : IPlaneService
    {
        private readonly DapperContext _context;
        public PlaneService(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<List<PlaneStockResponse>> GetPlaneStock(PlaneStockRequest rq)
        {
            int decryptedId = EncryptExtension.Decrypt(rq.UserId!);

            using (var connection = _context.CreateConnection())
            {
                string queryGetStock = $"select P.name as ProductName, PS.quantity as Quantity from attendants as A inner join planes as PL " +
                    $"on A.id = PL.id inner join planestock as PS on PL.id = PS.plane_id inner join products as P " +
                    $"on PS.product_id = P.id where A.user_id = {decryptedId}";

                var queryGetStockResponse = await connection.QueryAsync<PlaneStockResponse>(queryGetStock).ConfigureAwait(false);
                return queryGetStockResponse.ToList();
            }
        }
    }
}
