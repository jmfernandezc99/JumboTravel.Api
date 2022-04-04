namespace JumboTravel.Api.src.Domain.Models.Notifications
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Description { get; set; }
    }
}
