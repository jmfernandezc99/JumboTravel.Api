using JumboTravel.Api.src.Domain.Models.PlaneStocks.Requests;
using JumboTravel.Api.src.Domain.Models.PlaneStocks.Responses;

namespace JumboTravel.Api.src.Domain.Interfaces.Services
{
    public interface IPlaneService
    {
        Task<List<PlaneStockResponse>> GetPlaneStock(PlaneStockRequest rq);
    }
}
