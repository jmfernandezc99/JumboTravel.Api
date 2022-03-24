using Dapper;
using JumboTravel.Api.src.Application.Data;
using JumboTravel.Api.src.Domain.Interfaces.Services;
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
            var query = $"SELECT * FROM Users WHERE nif = '{rq.UserName}' AND password = '{rq.Password}'";
            using (var connection = _context.CreateConnection())
            {
                var response = await connection.QueryAsync<User>(query).ConfigureAwait(false);
                var user = response.FirstOrDefault();

                return user != null ? new LoginResponse()
                {
                    UserId = user.Id
                } : null;
            }
        }
    }
}
