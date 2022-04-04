using JumboTravel.Api.src.Domain.Models.Notifications;

namespace JumboTravel.Api.src.Domain.Interfaces.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetNotifications(string userId);
    }
}
