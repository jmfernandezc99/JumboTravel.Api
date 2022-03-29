namespace JumboTravel.Api.src.Domain.Models.Orders.Requests
{
    public class CreateOrderRequest
    {
        public string? UserId { get; set; }
        public List<CreateOrderProperties>? Properties { get; set; }
    }
}
