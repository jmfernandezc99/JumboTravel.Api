namespace JumboTravel.Api.src.Domain.Models.PlaneStocks.Responses
{
    public class GetPlaneStockResponse
    {
        public string Plane { get; set; } = string.Empty;
        public int Status { get; set; }
        public List<PlaneStockResponse> Stock { get; set; } = new List<PlaneStockResponse>();
    }
}
