namespace JumboTravel.Api.src.Domain.Models.Orders.Requests
{
    public class ObtainInvoiceResponse
    {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<ObtainInvoicePropertiesResponse>? Properties { get; set; }
    }
}
