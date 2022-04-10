namespace JumboTravel.Api.src.Domain.Models.Orders.Requests
{
    public class ObtainInvoicePropertiesResponse
    {
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
