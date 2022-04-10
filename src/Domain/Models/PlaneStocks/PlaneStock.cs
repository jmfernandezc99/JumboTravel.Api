namespace JumboTravel.Api.src.Domain.Models.PlaneStocks
{
    public class PlaneStock
    {
        public int PlaneId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int MaximumQuantity { get; set; }
    }
}
