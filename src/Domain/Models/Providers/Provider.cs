namespace JumboTravel.Api.src.Domain.Models.Providers
{
    public class Provider
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Base { get; set; } = string.Empty;
    }
}
