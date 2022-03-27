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
                string queryGetPlane = $"SELECT plane_id as PlaneId FROM attendants WHERE user_id = '{decryptedId}'";
                var queryGetPlaneResponse = await connection.QueryAsync<Attendant>(queryGetPlane).ConfigureAwait(false);
                var attendant = queryGetPlaneResponse.FirstOrDefault();

                string queryGetStock = $"SELECT plane_id as PlaneId, product_id AS ProductId, quantity FROM planestock WHERE plane_id = '{attendant!.PlaneId}'";

                var queryResponse = await connection.QueryAsync<PlaneStock>(queryGetStock).ConfigureAwait(false);
                var plainStocks = queryResponse.ToList();

                var response = new List<PlaneStockResponse>();

                foreach (var plainStock in plainStocks)
                {
                    string queryGetProductNames = $"SELECT name FROM products WHERE id = '{plainStock.ProductId}'";

                    var queryProductResponse = await connection.QueryAsync<Product>(queryGetProductNames).ConfigureAwait(false);
                    var product = queryProductResponse.FirstOrDefault();

                    if (product != null)
                    {
                        response.Add(new PlaneStockResponse()
                        {
                            ProductName = product.Name,
                            Quantity = plainStock.Quantity
                        });
                    }
                }

                return response;
            }
        }
    }
}
