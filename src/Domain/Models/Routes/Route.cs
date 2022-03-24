namespace JumboTravel.Api.src.Domain.Models.Routes
{
    public class Route
    {
        public int Id { get; set; }
        public int PlainId { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
    }
}
