namespace JumboTravel.Api.src.Domain.Models.Orders
{
    public class Order
    {
        public int Id { get; set; }
        public int AttendantId { get; set; }
        public int ProviderId { get; set; }
        public DateTime? Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
