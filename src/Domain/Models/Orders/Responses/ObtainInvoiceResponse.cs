namespace JumboTravel.Api.src.Domain.Models.Orders.Requests
{
    public class ObtainInvoiceResponse
    {
        public decimal TotalPrice { get; set; }
        public List<ObtainInvoicePropertiesResponse>? Properties { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
