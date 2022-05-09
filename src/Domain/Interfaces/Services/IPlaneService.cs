using JumboTravel.Api.src.Domain.Models.PlaneStocks.Responses;

namespace JumboTravel.Api.src.Domain.Interfaces.Services
{
    public interface IPlaneService
    {
        Task<GetPlaneStockResponse> GetPlaneStock(string authorization);
        Task<int> GetPlaneStatus(string authorization);
        Task<bool> ChangePlaneStatus(string authorization);
    }
}
