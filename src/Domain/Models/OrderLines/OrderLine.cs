namespace JumboTravel.Api.src.Domain.Models.OrderLines
{
    public class OrderLine
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
    }
}
