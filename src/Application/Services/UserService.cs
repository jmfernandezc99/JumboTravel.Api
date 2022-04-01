using Dapper;
using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Application.Extensions;
using JumboTravel.Api.src.Domain.Enums;
using JumboTravel.Api.src.Domain.Interfaces.Services;
using JumboTravel.Api.src.Domain.Models.Providers;
using JumboTravel.Api.src.Domain.Models.Users;
using JumboTravel.Api.src.Domain.Models.Users.Requests;
using JumboTravel.Api.src.Domain.Models.Users.Responses;

namespace JumboTravel.Api.src.Application.Services
{
    public class UserService : IUserService
    {
        private readonly DapperContext _context;
        public UserService(DapperContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<LoginResponse?> Login(LoginRequest rq)
        {
            using (var connection = _context.CreateConnection())
            {
                string query = $"SELECT * FROM Users WHERE nif = '{rq.UserName}' AND password = '{rq.Password}'";
                Role userRole = Role.Unknown;
                List<string> origins = new List<string>();

                var response = await connection.QueryAsync<User>(query).ConfigureAwait(false);
                var user = response.FirstOrDefault();

                if (user != null)
                {
                    string queryProvider = $"SELECT * FROM Providers WHERE user_id = '{user.Id}'";
                    var providers = await connection.QueryAsync<Provider>(queryProvider).ConfigureAwait(false);
                    var provider = providers.FirstOrDefault();
                    if (provider != null)
                    {
                        userRole = Role.Provider;
                        origins.Add(provider.Base);

                    } 
                    else
                    {
                        userRole = Role.Attendant;
                        string getOrigins = $"select R.origin from attendants as A inner join planes as p on A.plane_id = P.id inner join routes as R on R.plane_id = P.id where A.user_id = {user.Id} order by origin desc";
                        var getOriginsResponse = await connection.QueryAsync<string>(getOrigins).ConfigureAwait(false);
                        origins = getOriginsResponse.ToList();
                    }
                }

                return user != null ? new LoginResponse()
                {
                    UserId = EncryptExtension.Encrypt(user.Id),
                    Role = userRole,
                    Origins = origins
                } : null;
            }
        }
    }
}
