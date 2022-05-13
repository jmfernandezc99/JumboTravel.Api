namespace JumboTravel.Api.src.Domain.Models.Orders.Responses
{
    public class GetAllOrdersRegistryResponse
    {
        public string Attendant { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Plane { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
