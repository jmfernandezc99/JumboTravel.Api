namespace JumboTravel.Api.src.Domain.Models.PlainStocks
{
    public class PlainStock
    {
        public int PlainId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int MaximumQuantity { get; set; }
    }
}
