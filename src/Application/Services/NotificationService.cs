using Dapper;
using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Notifications;
using JumboTravel.Api.src.Domain.Models.Users;

namespace JumboTravel.Api.src.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly DapperContext _context;

        public NotificationService(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<List<Notification>> GetNotifications(string token)
        {
            User user = JwtExtension.ReturnUserFromToken(token);

            using (var connection = _context.CreateConnection())
            {
                string queryGetUser = $"select * from users where nif = '{user.Nif}'";
                var users = await connection.QueryAsync<User>(queryGetUser).ConfigureAwait(false);

                string queryGetNotifications = $"SELECT id, title, user_id as UserId, description from notifications where user_id = {users.FirstOrDefault()!.Id}";

                var getNotificationsResponse = await connection.QueryAsync<Notification>(queryGetNotifications).ConfigureAwait(false);
                return getNotificationsResponse.Count() > 0 ? getNotificationsResponse.ToList() : new List<Notification>();
            }
        }
    }
}
