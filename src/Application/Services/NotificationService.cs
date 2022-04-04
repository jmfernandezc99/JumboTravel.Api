using Dapper;
using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Notifications;

namespace JumboTravel.Api.src.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly DapperContext _context;

        public NotificationService(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<List<Notification>> GetNotifications(string userId)
        {
            using (var connection = _context.CreateConnection())
            {
                int decryptedId = EncryptExtension.Decrypt(userId);
                string queryGetNotifications = $"SELECT id, user_id as UserId, description from notifications where user_id = {decryptedId}";

                var getNotificationsResponse = await connection.QueryAsync<Notification>(queryGetNotifications).ConfigureAwait(false);
                return getNotificationsResponse.Count() > 0 ? getNotificationsResponse.ToList() : new List<Notification>();
            }
        }
    }
}
